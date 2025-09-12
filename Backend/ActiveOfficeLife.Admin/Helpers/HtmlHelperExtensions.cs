using Microsoft.AspNetCore.Mvc.Rendering;

namespace ActiveOfficeLife.Admin.Helpers
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Trả về "active" nếu controller/action khớp
        /// và (tuỳ chọn) query string khớp điều kiện trong paramTxt.
        /// - paramTxt: "key=value" (ví dụ: "s=published") hoặc chỉ "key" (chỉ cần có key).
        /// </summary>
        public static string IsActive(this IHtmlHelper htmlHelper, string controller, string action = null, string? paramTxt = "", string cssClass = "active")
        {
            var route = htmlHelper.ViewContext.RouteData.Values;
            var routeController = route["controller"]?.ToString();
            var routeAction = route["action"]?.ToString();

            // So khớp controller / action (không phân biệt hoa thường)
            if (!controller.Equals(routeController, StringComparison.OrdinalIgnoreCase))
                return string.Empty;

            if (!string.IsNullOrEmpty(action) &&
                !action.Equals(routeAction, StringComparison.OrdinalIgnoreCase))
                return string.Empty;

            // Nếu không yêu cầu check query → active
            if (string.IsNullOrWhiteSpace(paramTxt))
                return cssClass;

            // Parse "key=value" hoặc "key"
            string key, expected = null;
            var idx = paramTxt.IndexOf('=');
            if (idx > 0)
            {
                key = paramTxt[..idx].Trim();
                expected = paramTxt[(idx + 1)..].Trim();
            }
            else
            {
                key = paramTxt.Trim();
            }

            var req = htmlHelper.ViewContext.HttpContext.Request;
            if (!req.Query.TryGetValue(key, out var values))
                return string.Empty;

            // Nếu chỉ cần có key
            if (expected is null)
                return cssClass;

            // So sánh giá trị (lấy giá trị đầu nếu có nhiều)
            var actual = values.ToString();
            return actual.Equals(expected, StringComparison.OrdinalIgnoreCase) ? cssClass : string.Empty;
        }

        public static bool IsMenuOpen(this IHtmlHelper html, string controllers = null)
        {
            var routeData = html.ViewContext.RouteData;
            var currentController = routeData.Values["controller"]?.ToString();

            var acceptedControllers = string.IsNullOrEmpty(controllers) ? new[] { currentController } : controllers.Split(',');

            return acceptedControllers.Contains(currentController) ? true : false;
        }
    }
}
