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
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult All()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Published()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Drafts()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Closed()
        {
            return View();
        }
    }
}
