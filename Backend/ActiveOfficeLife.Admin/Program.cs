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
    });

var baseApi = builder.Configuration.GetSection("BaseApi").Get<BaseApi>();
var jwtSettings = builder.Configuration.GetSection("JwtTokens").Get<JwtTokens>();
var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";          // Redirect nếu chưa login
        options.AccessDeniedPath = "/denied";  // Nếu không có quyền
        options.ExpireTimeSpan = TimeSpan.FromHours(baseApi.AccessTokenExpireHours);
        options.SlidingExpiration = true;      // Tự kéo dài session
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

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<AuthMiddleware>();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
