using ActiveOfficeLife.Common.Enums;
using Microsoft.AspNetCore.Mvc;

namespace ActiveOfficeLife.Admin.Controllers
{
    public class ArticlesController : BaseController
    {
        public ArticlesController(IConfiguration configuration) : base(configuration)
        {
        }
        // GET: Article
        [HttpGet]
        public IActionResult Index([FromQuery] PostStatus s = PostStatus.All)
        {
            ViewData["status"] = s.ToString();
            return View();
        }
        
    }
}
