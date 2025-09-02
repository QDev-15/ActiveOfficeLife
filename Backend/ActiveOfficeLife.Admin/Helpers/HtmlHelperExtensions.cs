using Microsoft.AspNetCore.Mvc.Rendering;

namespace ActiveOfficeLife.Admin.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static string IsActive(this IHtmlHelper htmlHelper, string controller, string action = null)
        {
            var routeData = htmlHelper.ViewContext.RouteData;
            var routeController = routeData.Values["controller"]?.ToString()?.ToLower();
            var routeAction = routeData.Values["action"]?.ToString()?.ToLower();

            if (!string.IsNullOrEmpty(action))
                return (controller.ToLower() == routeController && action?.ToLower() == routeAction) ? "active" : "";

            return (controller.ToLower() == routeController) ? "active" : "";
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
