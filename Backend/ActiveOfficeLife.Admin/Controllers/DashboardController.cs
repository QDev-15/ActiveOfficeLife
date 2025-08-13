using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ActiveOfficeLife.Admin.Controllers
{
    public class DashboardController : BaseController
    {
        public DashboardController(IConfiguration configuration) : base(configuration)
        {
        }

        [AllowAnonymous]
        [HttpGet("/dashboard")]
        public IActionResult Index()
        {
            
            return View();
        }
    }
}
