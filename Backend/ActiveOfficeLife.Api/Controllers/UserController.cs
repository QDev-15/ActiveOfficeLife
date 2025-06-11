using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Application.Requests;
using ActiveOfficeLife.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ActiveOfficeLife.Api.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public UserController(IUserService userService, ITokenService token)
        {
            _userService = userService;
            _tokenService = token;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userService.Login(request);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid credentials." });
            }

            var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Username, user.Roles);
            //var refreshToken = _tokenService.GenerateRefreshToken(user, request.Remember);

            return Ok(new LoginResponse
            {
                AccessToken = accessToken,
                //RefreshToken = refreshToken,
                Username = user.Username,
                //Role = user.Role
            });
        }
    }
}
