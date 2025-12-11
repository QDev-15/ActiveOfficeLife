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
        public async Task<IActionResult> Edit(string id)
        {
            ViewBag.PostId = id;           // đưa id sang View để JS đọc
            return View();                 // KHÔNG gọi API trong MVC
        }
        public async Task<IActionResult> Form(string id)
        {
            var categoryResponse = await _apiService.GetAsync(AOLEndPoint.PostGetById + "/" + id);
            var post = await categoryResponse.ToModelAsync<PostModel>();
            var tagResponse = await _apiService.GetAsync(AOLEndPoint.TagGetAll);
            var tags = await tagResponse.ToModelAsync<List<TagModel>>();
            var categoryListResponse = await _apiService.GetAsync(AOLEndPoint.CategoryGetAll + "?pageSize=1000");
            var categories = await categoryListResponse.ToModelAsync<List<CategoryModel>>();
            var selectList = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
            ViewBag.Categories = selectList;
            ViewBag.AllTags = tags ?? new List<TagModel>();
            post!.TagIds = post.Tags?.Select(t => t.Id).ToList() ?? new List<Guid>();
            return View(post);
        }
        // /Articles/View/{idOrSlug}?preview=1
        public async Task<IActionResult> View(string idOrSlug, [FromQuery] bool preview = false)
        {
            var isGuid = Guid.TryParse(idOrSlug, out var gid);
            var url = isGuid
                ? $"{AOLEndPoint.PostGetById}/{gid}"
                : $"{AOLEndPoint.PostGetBySlug}/{Uri.EscapeDataString(idOrSlug)}";

            var resp = await _apiService.GetAsync(url);
            var post = await resp.ToModelAsync<PostModel>();
            if (post == null) return NotFound();

            // Không cho xem public nếu chưa Published (trừ khi preview=1)
            var isPublished = string.Equals(post.Status, "Published", StringComparison.OrdinalIgnoreCase);
            if (!isPublished && !preview) return NotFound();

            return View(post); // View ở /Views/Articles/View.cshtml
        }
    }
}
