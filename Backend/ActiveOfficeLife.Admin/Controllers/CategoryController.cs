using Microsoft.AspNetCore.Mvc;

namespace ActiveOfficeLife.Admin.Controllers
{
    public class CategoryController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Detail(string id)
        {
            return View();
        }
    }
}
