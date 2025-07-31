using ActiveOfficeLife.Common;
using ActiveOfficeLife.Common.AppConfigs;
using Microsoft.Extensions.Configuration;

namespace ActiveOfficeLife.Application.Services
{
    public class AppConfigService
    {
        public readonly AppConfigs AppConfigs;

        public AppConfigService(IConfiguration configuration)
        {
            AppConfigs = new AppConfigs();
            AppConfigs.HostingConfigs = configuration.GetSection("HostingConfig").Get<HostingConfigs>();
            AppConfigs.JwtTokens = configuration.GetSection("JwtTokens").Get<JwtTokens>();
            AppConfigs.ConnectionStrings = configuration.GetSection("ConnectionStrings").Get<ConnectionStrings>();
            AppConfigs.Sessions = configuration.GetSection("Sessions").Get<Sessions>();
            AppConfigs.EmailSmtp = configuration.GetSection("Email").Get<EmailSmtp>();
            AppConfigs.ApiUrl = configuration["ApiUrl"] ?? string.Empty;
            AppConfigs.ApplicationName = configuration["ApplicationName"] ?? string.Empty;
            AppConfigs.Version = configuration["Version"] ?? string.Empty;
            AppConfigs.Environment = configuration["Environment"] ?? string.Empty;
            AppConfigs.ApplicationUrl = configuration["ApplicationUrl"] ?? string.Empty;
            AppConfigs.EnableSwagger = bool.TryParse(configuration["EnableSwagger"], out var enableSwagger) && enableSwagger;
            AppConfigs.EnableCors = bool.TryParse(configuration["EnableCors"], out var enableCors) && enableCors;   
            AppConfigs.EnableAuthentication = bool.TryParse(configuration["EnableAuthentication"], out var enableAuthentication) && enableAuthentication;
            AppConfigs.EnableAuthorization = bool.TryParse(configuration["EnableAuthorization"], out var enableAuthorization) && enableAuthorization;
            AppConfigs.CacheTimeout = int.TryParse(configuration["CacheTimeout"], out var cacheTimeout) ? cacheTimeout : 30; // Default to 30 minutes if not set
        }
    }
}
