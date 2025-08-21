using ActiveOfficeLife.Api;
using ActiveOfficeLife.Application;
using ActiveOfficeLife.Application.Common;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Common.AppConfigs;
using ActiveOfficeLife.Domain.EFCore.DBContext;
using ActiveOfficeLife.Infrastructure;
using ActiveOfficeLife.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// define settings for the application
var environment = builder.Environment.EnvironmentName;
var jwtSettings = builder.Configuration.GetSection("JwtTokens").Get<JwtTokens>();
var connectionSettings = builder.Configuration.GetSection("ConnectionStrings").Get<ConnectionStrings>();
var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
var connectionString = connectionSettings.DefaultConnection;


builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();


// Register JwtTokens configuration for dependency injection: ex using IOptions<JwtTokens> on your services
builder.Services.Configure<JwtTokens>(builder.Configuration.GetSection("JwtTokens"));

builder.Services.AddDbContext<ActiveOfficeLifeDbContext>(options =>
    options.UseSqlServer(connectionString));
// Add services to the container.


// Thêm Authentication - JWT Bearer
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // chỉ false trong dev
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true, // ✅ Tự động từ chối token hết hạn
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero // Không cho phép trễ thời gian
    };
    // ✅ Hook sự kiện kiểm tra lỗi token
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception is SecurityTokenExpiredException)
            {
                context.Response.Headers.Add("Token-Expired", "true");

                // Bạn có thể log, hoặc ghi log ở đây
                Console.WriteLine("⚠️ Token đã hết hạn.");
            }

            return Task.CompletedTask;
        }
    };
});

// Cho phép tất cả CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder
            .AllowAnyOrigin()         // Cho tất cả domain
            .AllowAnyMethod()         // Cho tất cả method (GET, POST, PUT, DELETE,...)
            .AllowAnyHeader();        // Cho tất cả header
    });
});

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddMemoryCache(); // Thêm bộ nhớ cache nếu cần
builder.Services.AddSingleton<CustomMemoryCache>();
// Thêm các dịch vụ ứng dụng
builder.Services.AddActiveOfficeLifeInfrastructure();
builder.Services.AddActiveOfficeLifeApplication();

builder.Services.AddHostedService<LogBackgroundService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "AOL API", Version = "v1" });

    // Thêm phần cấu hình xác thực Bearer token
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập token theo dạng: Bearer {your token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


var app = builder.Build();
// setup ILogService for static helper
//var logService = app.Services.GetRequiredService<ILogService>();
//AOLLogger.LogService = logService;
// ✅ ILogService từ DI container
using (var scope = app.Services.CreateScope())
{
    var logService = scope.ServiceProvider.GetRequiredService<ILogService>();
    AOLLogger.Initialize(logService); // ✅ Gán vào static logger
}
// Apply EF Core migrations tự động khi khởi động
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<ActiveOfficeLifeDbContext>();
        dbContext.Database.Migrate(); // ✅ tự động tạo database & apply migrations
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error applying migrations: " + ex.Message);
        // Log lỗi hoặc throw lại
    }
}



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();           // Xác định endpoint
app.UseCors("AllowAll");    // Gắn header CORS cho response
app.Use(async (context, next) =>
{
    if (context.Request.Method == HttpMethods.Options)
    {
        context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
        context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
        context.Response.StatusCode = 200;
        await context.Response.CompleteAsync();
    }
    else
    {
        await next();
    }
});
app.UseAuthentication();    // Xác thực (JWT, cookie, v.v.)
app.UseAuthorization();     // Phân quyền
app.MapControllers();       // Map đến controller
app.UseMiddleware<TokenValidationMiddleware>();

app.Run();
