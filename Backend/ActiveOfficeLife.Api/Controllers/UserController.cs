using ActiveOfficeLife.Application.Common;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Application.Services;
using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Common.Requests;
using ActiveOfficeLife.Common.Responses;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace ActiveOfficeLife.Api.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly CustomMemoryCache _cache;
        private readonly AppConfigService _appConfigService;
        private readonly IEmailService _emailService;

        public UserController(IUserService userService, ITokenService token, CustomMemoryCache cache, AppConfigService appConfigService, IEmailService emailService)
        {
            _appConfigService = appConfigService;
            _userService = userService;
            _tokenService = token;
            _cache = cache;
            _emailService = emailService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromBody] PagingRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.SearchText))
                return BadRequest(new ResultError("Invalid search request."));
            try
            {
                var users = await _userService.GetAll(request.SearchText.Trim(), request.PageIndex, request.PageSize, request.SortDirection.ToLower() == "desc");
                return Ok(new ResultSuccess(users));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error searching users: {ex.Message}");
                return BadRequest(new ResultError("Failed to search users."));
            }
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

        [HttpGet("getbyusername")]
        public async Task<IActionResult> GetUserByUsername([FromQuery] string username)
        {
            if (string.IsNullOrEmpty(username))
                return BadRequest(new ResultError("Username cannot be empty."));
            try
            {
                var user = await _userService.GetByUsername(username.Trim());
                if (user == null)
                    return NotFound(new ResultError("User not found."));
                return Ok(new ResultSuccess(user));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error fetching user by username: {ex.Message}");
                return BadRequest(new ResultError("Failed to retrieve user."));
            }
        }
        [HttpGet("getbyemail")]
        public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest(new ResultError("Email cannot be empty."));
            try
            {
                var user = await _userService.GetByEmail(email.Trim());
                if (user == null)
                    return NotFound(new ResultError("User not found."));
                return Ok(new ResultSuccess(user));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error fetching user by email: {ex.Message}");
                return BadRequest(new ResultError("Failed to retrieve user."));
            }
        }
        [HttpGet("getbyphonenumber")]
        public async Task<IActionResult> GetUserByPhoneNumber([FromQuery] string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return BadRequest(new ResultError("Phone number cannot be empty."));
            try
            {
                var user = await _userService.GetByPhoneNumber(phoneNumber.Trim());
                if (user == null)
                    return NotFound(new ResultError("User not found."));
                return Ok(new ResultSuccess(user));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error fetching user by phone number: {ex.Message}");
                return BadRequest(new ResultError("Failed to retrieve user."));
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
                var users = await _userService.GetAll(string.Empty, page, pageSize, isDesc);
                return Ok(new ResultSuccess(users));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error fetching all users: {ex.Message}");
                return BadRequest(new ResultError("Failed to retrieve users."));
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

        [AllowAnonymous]
        [HttpPost("forgotpassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email))
                return BadRequest(new ResultError("Invalid forgot password request."));
            try
            {
                var user = await _userService.ForgotPassword(request);
                if (user == null)
                    return NotFound(new ResultError("Email not found."));
                // Optionally, you can send an email with the reset link here
                var token = await _tokenService.GeneratePasswordResetTokenAsync();
                var encodedToken = HttpUtility.UrlEncode(token);

                var resetLink = $"{_appConfigService.AppConfigs.ApiUrl}/resetpassword?email={request.Email}&token={encodedToken}";

                var emailBody = $@"
                                <p>Click the link below to reset your password:</p>
                                <a href='{resetLink}'>Reset Password</a>";

                await _emailService.SendEmailAsync(user.Email, "Reset Your Password", emailBody);

                return Ok(new ResultSuccess("Password reset link sent to your email."));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error processing forgot password: {ex.Message}");
                return BadRequest(new ResultError("Failed to process forgot password request."));
            }
        }
        [HttpPut("update")]
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
        
        [HttpPatch("changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.OldPassword) || string.IsNullOrEmpty(request.NewPassword))
                return BadRequest(new ResultError("Invalid password change request."));
            try
            {
                var userId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new ResultError("User not authenticated."));
                var result = await _userService.ChangePassword(Guid.Parse(userId), request);
                if (!result)
                    return BadRequest(new ResultError("Failed to change password."));
                // Clear cache for the user after password change
                _cache.Remove($"UserProfile_{userId}");
                return Ok(new ResultSuccess("Password changed successfully."));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error changing password: {ex.Message}");
                return BadRequest(new ResultError("Failed to change password."));
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
