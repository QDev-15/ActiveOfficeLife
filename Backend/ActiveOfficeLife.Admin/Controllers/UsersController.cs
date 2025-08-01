using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ActiveOfficeLife.Admin.Controllers
{
    public class UsersController : BaseController
    {
        public UsersController(IConfiguration configuration) : base(configuration)
        {
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpGet("/forgot-password")]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpGet("/logout")]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete(baseApi.AccessToken);
            // Xoá thông tin người dùng khỏi session
            HttpContext.Session.Remove("userinfo");
            HttpContext.Session.Remove("username");
            HttpContext.Session.Remove("fullname");
            HttpContext.Session.Remove("role");
            // Xoá cookie xác thực
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }

    }
}
