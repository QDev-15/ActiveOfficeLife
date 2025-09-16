using ActiveOfficeLife.Admin.Interfaces;
using ActiveOfficeLife.Common;
using ActiveOfficeLife.Common.Enums;
using ActiveOfficeLife.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Admin.Controllers
{
    public class ArticlesController : BaseController
    {
        private readonly IApiService _apiService;
        public ArticlesController(IConfiguration configuration, IApiService apiService) : base(configuration)
        {
            _apiService = apiService;
        }
        // GET: Article
        [HttpGet]
        public IActionResult Index([FromQuery] PostStatus s = PostStatus.All)
        {
            ViewData["status"] = s.ToString();
            return View();
        }
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var modelResponse = await _apiService.GetAsync(AOLEndPoint.PostGetById + "/" + id); /* lấy từ DB theo id (Status mặc định Draft) */;
            var model = await modelResponse.ToModelAsync<PostModel>() ?? new PostModel();
            var modelCategoriesResponse = await _apiService.GetAsync(AOLEndPoint.CategoryGetAll + "?pageSize=10000");
            var categories = await modelCategoriesResponse.ToModelAsync<List<CategoryModel>>() ?? new List<CategoryModel>();

            // Nạp SelectList
            ViewBag.Categories = new SelectList(categories, "Id", "Name", model.CategoryId);
            ViewBag.AllTags = allTags; // nếu dùng
            return View(model);
        }

    }
}
