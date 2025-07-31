using ActiveOfficeLife.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ActiveOfficeLife.Admin.Controllers
{
    public class HomeController : BaseController
    {

        public HomeController(IConfiguration configuration) : base(configuration)
        {
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpGet("forgot-password")]
        public IActionResult ForgotPassword()
        {
            return View();
        }


        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
