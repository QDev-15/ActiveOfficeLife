using ActiveOfficeLife.Admin.Models;
using ActiveOfficeLife.Common.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

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
            if (TempData["UserInfo"] != null)
            {
                var userJson = TempData["UserInfo"].ToString();
                var user = JsonSerializer.Deserialize<UserModel>(userJson);

                ViewBag.Username = user.Username;
                ViewBag.Fullname = user.FullName ?? user.Username;
                ViewBag.Roles = string.Join(",", user.Roles);
                ViewBag.AccessToken = Request.Cookies[baseApi.AccessToken];
                ViewBag.RefreshToken = TempData["refresh_token"]?.ToString();
            }
            else
            {
                // fallback từ session nếu F5 hoặc trở lại sau login
                ViewBag.Username = HttpContext.Session.GetString("username") ?? "guest";
                ViewBag.Fullname = HttpContext.Session.GetString("fullname") ?? "guest";
                ViewBag.Roles = HttpContext.Session.GetString("role") ?? "";
                ViewBag.AccessToken = Request.Cookies[baseApi.AccessToken];
            }


            return View();
        }

        [HttpGet]
        public IActionResult Privacy()
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
