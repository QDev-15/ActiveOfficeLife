// Middlewares/RouteRewriteMiddleware.cs
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Admin.Middlewares
{
    /// <summary>
    /// Middleware áp dụng các luật rewrite/redirect cho route
    /// </summary>
    public class RouteRewriteMiddleware
    {
        private readonly RequestDelegate _next;

        public RouteRewriteMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var req = context.Request;

            // Match chính xác /Articles (không phân biệt hoa/thường), và KHÔNG có query s
            if (req.Path.Equals("/Articles", StringComparison.OrdinalIgnoreCase) &&
                !req.Query.Keys.Any())
            {
                var qb = new QueryString();
                
                qb = qb.Add("s", "all");

                var target = $"{req.Path}{qb}";
                context.Response.Redirect(target, permanent: false);
                return;
            }

            await _next(context);
        }

    }

    public static class RouteRewriteExtensions
    {
        /// <summary>
        /// Đăng ký middleware rewrite/redirect route tùy biến
        /// </summary>
        public static IApplicationBuilder UseRouteRewrite(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RouteRewriteMiddleware>();
        }
    }
}
