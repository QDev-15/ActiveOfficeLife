using ActiveOfficeLife.Common;
using Microsoft.AspNetCore.Authorization;
using System.Configuration;

namespace ActiveOfficeLife.Admin.Middlewares
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IConfiguration configuration)
        {
            BaseApi baseApi = configuration.GetSection("BaseApi").Get<BaseApi>();
            // Lấy endpoint hiện tại
            var endpoint = context.GetEndpoint();

            // Nếu endpoint không yêu cầu Authorize => bỏ qua middleware
            var hasAuthorize = endpoint?.Metadata?.GetMetadata<AuthorizeAttribute>() != null;
            var hasAllowAnonymous = endpoint?.Metadata?.GetMetadata<AllowAnonymousAttribute>() != null;

            if (hasAuthorize && !hasAllowAnonymous)
            {
                var pathLogin = context.Request.Path.Value?.ToLower();
                if (!pathLogin.Contains("/login"))
                {
                    var token = context.Request.Cookies[baseApi.AccessToken];
                    if (string.IsNullOrEmpty(token))
                    {
                        context.Response.Redirect("/login");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }

}
