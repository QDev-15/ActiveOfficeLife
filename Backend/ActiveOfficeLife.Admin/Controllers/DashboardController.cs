using Microsoft.AspNetCore.Mvc;

namespace ActiveOfficeLife.Admin.Controllers
{
    public class DashboardController : BaseController
    {
        public DashboardController(IConfiguration configuration) : base(configuration)
        {
        }

        [HttpGet("/dashboard")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
