using Microsoft.AspNetCore.Mvc;

namespace ActiveOfficeLife.Admin.Controllers
{
    public class SettingsController : BaseController
    {
        public SettingsController(IConfiguration configuration) : base(configuration)
        {
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
