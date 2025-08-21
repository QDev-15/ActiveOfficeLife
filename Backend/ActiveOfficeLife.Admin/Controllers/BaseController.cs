using ActiveOfficeLife.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ActiveOfficeLife.Admin.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        protected BaseApi baseApi;
        public BaseController(IConfiguration configuration)
        {
            baseApi = configuration.GetSection("BaseApi").Get<BaseApi>();
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewData["title"] = "AOL Admin";
            ViewData["apiUrl"] = baseApi?.Url;
            ViewData["accessTokenKey"] = baseApi?.AccessToken;

            base.OnActionExecuting(context);
        }
    }
}
