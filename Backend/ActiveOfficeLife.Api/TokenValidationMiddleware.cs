using ActiveOfficeLife.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace ActiveOfficeLife.Api
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUserTokenRepository tokenRepository)
        {
            // ✅ Bỏ qua preflight CORS request
            if (context.Request.Method.Equals(HttpMethods.Options, StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            // Lấy endpoint hiện tại
            var endpoint = context.GetEndpoint();

            // Nếu endpoint không yêu cầu Authorize => bỏ qua middleware
            var hasAuthorize = endpoint?.Metadata?.GetMetadata<AuthorizeAttribute>() != null;
            var hasAllowAnonymous = endpoint?.Metadata?.GetMetadata<AllowAnonymousAttribute>() != null;

            if (hasAuthorize && !hasAllowAnonymous)
            {
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                if (!string.IsNullOrEmpty(token))
                {
                    var tokenExists = await tokenRepository.IsValidAccessTokenAsync(token);
                    if (!tokenExists)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Token is revoked or invalid");
                        return;
                    }
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Missing Authorization header");
                    return;
                }
            }

            await _next(context);
        }
    }

}
