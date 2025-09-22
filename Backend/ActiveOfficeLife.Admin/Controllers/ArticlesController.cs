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

    }
}
