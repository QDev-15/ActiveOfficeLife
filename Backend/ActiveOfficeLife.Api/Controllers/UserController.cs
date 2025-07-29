using ActiveOfficeLife.Application.Common;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Application.Models;
using ActiveOfficeLife.Application.Models.Requests;
using ActiveOfficeLife.Application.Models.Responses;
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

        [HttpGet("getuser")]
        public async Task<IActionResult> GetProfile([FromQuery] bool noCache = false)
        {
            var userId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ResultError("User not authenticated."));

            var cacheKey = $"UserProfile_{userId}";

            try
            {
                if (!noCache)
                {
                    var cachedUser = _cache.Get<UserModel>(cacheKey);
                    if (cachedUser != null)
                        return Ok(new ResultSuccess(cachedUser));
                }

                var user = await _userService.GetUser(Guid.Parse(userId));
                if (user == null)
                    return NotFound(new ResultError("User not found."));

                _cache.Set(cacheKey, user, TimeSpan.FromMinutes(_appConfigService.AppConfigs.CacheTimeout)); // tùy TTL
                return Ok(new ResultSuccess(user));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error fetching user profile: {ex.Message}");
                return BadRequest(new ResultError("Failed to retrieve user profile."));
            }
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new ResultError("Invalid registration data."));
            }
            try
            {
                request.Email = request.Email?.Trim() ?? string.Empty;
                request.Username = request.Username.Trim();
                request.Password = request.Password.Trim();
                var user = await _userService.Create(request);
                return Ok(new ResultSuccess(user));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error trimming registration data: {ex.Message}");
                return BadRequest(new ResultError("User registration failed."));
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
                    return Unauthorized(new ResultError("User not authenticated."));
                }
                var histories = await _tokenService.GetUserTokensAsync(Guid.Parse(userId));
                return Ok(new ResultSuccess(histories));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error fetching login history: {ex.Message}");
                return BadRequest(new ResultError("Failed to retrieve login history."));
            }

        }
        [HttpGet("getbyid")]
        public async Task<IActionResult> GetUserById([FromQuery] Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest(new ResultError("Invalid user ID."));
            try
            {
                var user = await _userService.GetUser(userId);
                if (user == null)
                    return NotFound(new ResultError("User not found."));
                return Ok(new ResultSuccess(user));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error fetching user by ID: {ex.Message}");
                return BadRequest(new ResultError("Failed to retrieve user."));
            }
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string sort = "desc")
        {
            try
            {
                var isDesc = sort?.ToLower() == "desc";
                var users = await _userService.GetAll(page, pageSize, isDesc);
                return Ok(new ResultSuccess(users));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error fetching all users: {ex.Message}");
                return BadRequest(new ResultError("Failed to retrieve users."));
            }
        }
        [HttpPost("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UserModel userModel)
        {
            if (userModel == null || userModel.Id == Guid.Empty)
                return BadRequest(new ResultError("Invalid user data."));
            try
            {
                var updatedUser = await _userService.Update(userModel);
                if (updatedUser == null)
                    return NotFound(new ResultError("User not found."));
                // Clear cache for the updated user
                _cache.Remove($"UserProfile_{userModel.Id}");
                return Ok(new ResultSuccess(updatedUser));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error updating user: {ex.Message}");
                return BadRequest(new ResultError("Failed to update user."));
            }
        }
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteUser([FromQuery] Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest(new ResultError("Invalid user ID."));
            try
            {
                var result = await _userService.Delete(userId);
                if (!result)
                    return NotFound(new ResultError("User not found."));
                // Clear cache for the deleted user
                _cache.Remove($"UserProfile_{userId}");
                return Ok(new ResultError("User deleted successfully."));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error deleting user: {ex.Message}");
                return BadRequest(new ResultError("Failed to delete user."));
            }
        }
    }
}
