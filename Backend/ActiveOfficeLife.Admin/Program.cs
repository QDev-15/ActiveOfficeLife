using ActiveOfficeLife.Admin.FluentValidations;
using ActiveOfficeLife.Admin.Interfaces;
using ActiveOfficeLife.Admin.Middlewares;
using ActiveOfficeLife.Admin.Services;
using ActiveOfficeLife.Common;
using ActiveOfficeLife.Common.AppConfigs;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IApiService, ApiService>();
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation()
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblyContaining<LoginRequestValidator>();
        fv.RegisterValidatorsFromAssemblyContaining<RegisterRequestValidator>();
        fv.RegisterValidatorsFromAssemblyContaining<CategoryModelValidator>();
        fv.DisableDataAnnotationsValidation = true; // Optional
    });

var baseApi = builder.Configuration.GetSection("BaseApi").Get<BaseApi>();

builder.Services.AddDistributedMemoryCache(); // ✅ thêm dòng này trước AddSession

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(baseApi.AccessTokenExpireHours);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Nếu bạn dùng HTTPS
        options.Cookie.SameSite = SameSiteMode.Lax;               // Đảm bảo cookie gửi khi điều hướng
        options.LoginPath = "/dashboard";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/denied";
        options.ExpireTimeSpan = TimeSpan.FromHours(baseApi.AccessTokenExpireHours);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

//app.UseMiddleware<AuthMiddleware>();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
