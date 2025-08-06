using ActiveOfficeLife.Admin.Interfaces;
using ActiveOfficeLife.Common;
using ActiveOfficeLife.Common.Requests;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Admin.Controllers
{
    public class UsersController : BaseController
    {
        private readonly IApiService _apiService;
        public UsersController(IConfiguration configuration, IApiService apiService) : base(configuration)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpGet("/signup")]
        public IActionResult Signup()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost("/signup")]
        public async Task<IActionResult> Signup(RegisterRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.Email))
            {
                ViewBag.Error = "Username, password, and email are required.";
                return View(request);
            }
            if (request.Password != request.ConfirmPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                return View(request);
            }
            var response = await _apiService.PostAsync(AOLEndPoint.UserRegister, request);
            if (response != null && response.IsSuccessStatusCode)
            {
                ViewBag.Success = "Registration successful. You can now log in.";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.Error = "Registration failed. Please try again.";
                return View(request);
            }
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
