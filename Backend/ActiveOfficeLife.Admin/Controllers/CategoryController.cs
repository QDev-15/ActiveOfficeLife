using ActiveOfficeLife.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace ActiveOfficeLife.Admin.Controllers
{
    public class CategoryController : BaseController
    {
        public CategoryController(IConfiguration configuration) : base(configuration)
        {
        }

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
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(CategoryModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Trả lại form kèm lỗi
            }
            // Logic to create a new category
            // Redirect to the index or detail page after creation
            return RedirectToAction("Index");
        }
    }
}
