using Microsoft.AspNetCore.Authorization;

namespace ActiveOfficeLife.Admin.Middlewares
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            


            // Lấy endpoint hiện tại
            var endpoint = context.GetEndpoint();

            // Nếu endpoint không yêu cầu Authorize => bỏ qua middleware
            var hasAuthorize = endpoint?.Metadata?.GetMetadata<AuthorizeAttribute>() != null;
            var hasAllowAnonymous = endpoint?.Metadata?.GetMetadata<AllowAnonymousAttribute>() != null;

            if (hasAuthorize && !hasAllowAnonymous)
            {
                var path = context.Request.Path.Value?.ToLower();
                if (!path.Contains("/login") && !path.Contains("/auth"))
                {
                    var token = context.Request.Cookies["access_token"];
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
