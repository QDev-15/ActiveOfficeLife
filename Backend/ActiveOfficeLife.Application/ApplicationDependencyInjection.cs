using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddActiveOfficeLifeApplication(this IServiceCollection services)
        {
            // Register application services here
            // Example: services.AddScoped<ICategoryService, CategoryService>();
            // Add other application services as needed
            services.AddSingleton<AppConfigService>();
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserService, UserService>();
            return services;

        }
    }
}
