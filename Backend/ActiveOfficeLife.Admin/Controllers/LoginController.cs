using ActiveOfficeLife.Admin.Interfaces;
using ActiveOfficeLife.Common;
using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Common.Requests;
using ActiveOfficeLife.Common.Responses;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Admin.Controllers
{
    public class LoginController : BaseController
    {
        private readonly IApiService _apiService;

        public LoginController(IApiService apiService, IConfiguration configuration) : base(configuration)
        {
            _apiService = apiService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Index(LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
            {
                ViewBag.Error = "Username and password are required.";
                return View(request);
            }
            var response = await _apiService.PostAsync(AOLEndPoint.AuthLogin, request);

            if (response != null && response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(responseBody);
                var data = jsonDoc.RootElement.GetProperty("data").ToString();
                var auth = JsonSerializer.Deserialize<AuthResponse>(data, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
                
                Response.Cookies.Append(baseApi.AccessToken, auth.AccessToken, new CookieOptions
                {
                    HttpOnly = false,
                    Secure = Request.IsHttps, // ✔ tự động true nếu đang chạy HTTPS
                    SameSite = SameSiteMode.Lax, // ✔ Cho phép redirect sau login
                    Expires = DateTimeOffset.UtcNow.AddHours(baseApi.AccessTokenExpireHours),
                    IsEssential = true,
                });

                // Lưu thông tin người dùng vào session (nếu muốn dùng server-side)

                // get user info
                var userResponse = await _apiService.GetAsync(AOLEndPoint.AuthMe, auth.AccessToken);
                if (userResponse != null && userResponse.IsSuccessStatusCode)
                {
                    var userBody = await userResponse.Content.ReadAsStringAsync();
                    var jsonUser = JsonDocument.Parse(userBody);
                    var userData = jsonUser.RootElement.GetProperty("data").ToString();
                    var user = JsonSerializer.Deserialize<UserModel>(userData, new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    Response.Cookies.Append("userinfo", JsonSerializer.Serialize(user), new CookieOptions
                    {
                        HttpOnly = false,
                        Secure = Request.IsHttps, // ✔ tự động true nếu đang chạy HTTPS
                        SameSite = SameSiteMode.Lax, // ✔ Cho phép redirect sau login
                        Expires = DateTimeOffset.UtcNow.AddHours(baseApi.AccessTokenExpireHours),
                        IsEssential = true,
                    });

                    // 3. Tạo claims
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user?.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.FullName ?? user.Username),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, string.Join(',', user.Roles)) // nếu nhiều role thì dùng multiple claim
                    };

                    // 4. Tạo identity và đăng nhập
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Error = "Login failed";
            return View("Index", request);
        }

        
    }
}
