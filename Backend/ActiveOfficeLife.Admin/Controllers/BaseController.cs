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
            ViewData["title"] = "AOL Admin";
        }
    }
}
