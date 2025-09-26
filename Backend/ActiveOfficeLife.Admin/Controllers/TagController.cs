using ActiveOfficeLife.Admin.Services;
using ActiveOfficeLife.Common;
using ActiveOfficeLife.Common.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Admin.Controllers
{
    public class TagController : BaseController
    {
        private readonly string serviceName = "TagService";
        private readonly ApiService _apiService;
        public TagController(IConfiguration configuration, ApiService apiService) : base(configuration)
        {
            _apiService = apiService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("/Tag/Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            ViewBag.TagId = id;           // đưa id sang View để JS đọc
            var tagresponse = await _apiService.GetAsync($"{AOLEndPoint.TagGetById}?id={id}");
            var tag = await tagresponse.ToModelAsync<TagModel>();
            return View(tag);
        }
    }
}
