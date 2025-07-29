using ActiveOfficeLife.Domain.Interfaces;

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

            await _next(context);
        }
    }

}
