using ActiveOfficeLife.Application;
using ActiveOfficeLife.Application.Common;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Application.Services;
using ActiveOfficeLife.Common;
using ActiveOfficeLife.Domain.EFCore.DBContext;
using ActiveOfficeLife.Domain.Interfaces;
using ActiveOfficeLife.Infrastructure;
using ActiveOfficeLife.Infrastructure.Repositories;
using ActiveOfficeLife.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ActiveOfficeLifeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionLocal")));
// Add services to the container.
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

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
});
builder.Services.AddAuthorization();


builder.Services.AddMemoryCache(); // Thêm bộ nhớ cache nếu cần
builder.Services.AddSingleton<CustomMemoryCache>();
// Thêm các dịch vụ ứng dụng
builder.Services.AddActiveOfficeLifeInfrastructure(builder.Configuration);
builder.Services.AddActiveOfficeLifeApplication();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
// setup ILogService for static helper
//var logService = app.Services.GetRequiredService<ILogService>();
//AOLLogger.LogService = logService;

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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Thêm middleware xác thực trước Authorization
app.UseAuthorization();
app.UseRouting();
app.MapControllers();

app.Run();
