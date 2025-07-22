using ActiveOfficeLife.Application.Common;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Application.Models;
using ActiveOfficeLife.Application.Models.Requests;
using ActiveOfficeLife.Application.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Api.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly CustomMemoryCache _cache;
        private readonly AppConfigService _appConfigService;

        public UserController(IUserService userService, ITokenService token, CustomMemoryCache cache, AppConfigService appConfigService)
        {
            _appConfigService = appConfigService;
            _userService = userService;
            _tokenService = token;
            _cache = cache;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile([FromQuery] bool noCache = false) {
            var userId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not authenticated." });

            var cacheKey = $"UserProfile_{userId}";

            try
            {
                if (!noCache)
                {
                    var cachedUser = _cache.Get<UserModel>(cacheKey);
                    if (cachedUser != null)
                        return Ok(cachedUser);
                }

                var user = await _userService.GetUser(Guid.Parse(userId));
                if (user == null)
                    return NotFound(new { message = "User not found." });

                _cache.Set(cacheKey, user, TimeSpan.FromMinutes(_appConfigService.AppConfigs.CacheTimeout)); // tùy TTL
                return Ok(user);
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error fetching user profile: {ex.Message}");
                return BadRequest(new { message = "Failed to retrieve user profile." });
            }
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Invalid registration data." });
            }
            try
            {
                request.Email = request.Email?.Trim() ?? string.Empty;
                request.Username = request.Username.Trim();
                request.Password = request.Password.Trim();
                var user = await _userService.Create(request);
                return Ok(user);
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error trimming registration data: {ex.Message}");
                return BadRequest(new { message = "User registration failed." });
            }
        }

        [HttpGet("login-history")]
        public async Task<IActionResult> GetLoginHistory()
        {
            try
            {
                var userId = User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User not authenticated." });
                }
                var histories = await _tokenService.GetUserTokensAsync(Guid.Parse(userId));
                return Ok(histories);
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error fetching login history: {ex.Message}");
                return BadRequest(new { message = "Failed to retrieve login history." });
            }

        }
    }
}
