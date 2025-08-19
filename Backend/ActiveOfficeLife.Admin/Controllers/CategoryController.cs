using ActiveOfficeLife.Admin.Interfaces;
using ActiveOfficeLife.Common;
using ActiveOfficeLife.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Admin.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly IApiService _apiService;
        public CategoryController(IConfiguration configuration, IApiService apiService) : base(configuration)
        {
            _apiService = apiService;
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
        public async Task<IActionResult> Create()
        {
            var response = await _apiService.GetAsync(Common.AOLEndPoint.CategoryGetAll);
            var parents = await response.ToModelAsync<List<CategoryModel>>() ?? new List<CategoryModel>();

            ViewBag.ParentCategories = parents
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToList();

            var newCategory = new CategoryModel
            {
                Id = Guid.NewGuid(),
                Name = string.Empty,
                Slug = Guid.NewGuid().ToString(), // Hoặc tạo slug từ tên nếu cần
                Description = string.Empty,
                ParentId = null // Hoặc gán giá trị mặc định nếu cần
            };
            return View("CreateOrUpdate", newCategory);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var response = await _apiService.GetAsync($"{Common.AOLEndPoint.CategoryGetById}?id={id}");
            var category = await response.ToModelAsync<CategoryModel>();
            if (category == null)
            {
                return NotFound();
            }
            var parentsResponse = await _apiService.GetAsync(Common.AOLEndPoint.CategoryGetAll + "?pageSize=1000");
            var parents = await parentsResponse.ToModelAsync<List<CategoryModel>>() ?? new List<CategoryModel>();
            ViewBag.ParentCategories = parents
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = c.Id == category.ParentId // Chọn category cha nếu có
                })
                .ToList();
            return View("CreateOrUpdate", category);
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
