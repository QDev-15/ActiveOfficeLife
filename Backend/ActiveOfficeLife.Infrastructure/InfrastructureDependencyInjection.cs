﻿using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Domain.Interfaces;
using ActiveOfficeLife.Infrastructure.Repositories;
using ActiveOfficeLife.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Infrastructure
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddActiveOfficeLifeInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Các config khác...
            services.AddScoped<_IUnitOfWork, _UnitOfWork>();
            services.AddScoped<IAdRepository, AdRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ILogRepository, LogRepository>();
            services.AddScoped<IMediaRepository, MediaRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<ISeoMetadataRepository, SeoMetadataRepository>();
            services.AddScoped<ISettingRepository, SettingRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IVisitorRepository, VisitorRepository>();
            services.AddScoped<IUserTokenRepository, UserTokenRepository>();
            return services;
        }
    }
}
