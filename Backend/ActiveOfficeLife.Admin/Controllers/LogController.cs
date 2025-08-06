using Microsoft.AspNetCore.Mvc;

namespace ActiveOfficeLife.Admin.Controllers
{
    public class LogController : BaseController
    {
        public LogController(IConfiguration configuration) : base(configuration)
        {
            ViewData["title"] = "AOL Admin - Logs";
        }
        [HttpGet("/logs")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
