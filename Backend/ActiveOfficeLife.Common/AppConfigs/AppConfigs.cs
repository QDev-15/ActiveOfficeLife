
namespace ActiveOfficeLife.Common.AppConfigs
{
    public class AppConfigs
    {
        public ConnectionStrings ConnectionStrings { get; set; } = new ConnectionStrings();
        public JwtTokens JwtTokens { get; set; } = new JwtTokens();
        public HostingConfigs HostingConfigs { get; set; } = new HostingConfigs();
        public Sessions Sessions { get; set; } = new Sessions();
        public EmailSmtp EmailSmtp { get; set; } = new EmailSmtp();
        public string ApiUrl { get; set; } = string.Empty;
        public string WebAppUrl { get; set; } = string.Empty;
        public string AdminAppUrl { get; set; } = string.Empty;
        public string ApplicationName { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public string ApplicationUrl { get; set; } = string.Empty;
        public bool EnableSwagger { get; set; }
        public bool EnableCors { get; set; }
        public bool EnableAuthentication { get; set; }
        public bool EnableAuthorization { get; set; }
        public int CacheTimeout { get; set; } = 30; // Default cache timeout in minutes
    }
}
