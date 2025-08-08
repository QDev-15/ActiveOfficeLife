using ActiveOfficeLife.Application.Common;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Common.Enums;
using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Common.Requests;
using ActiveOfficeLife.Common.Responses;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ActiveOfficeLife.Api.Controllers
{
    public class AuthController : BaseController
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        public AuthController(ITokenService tokenService, IUserService userService)
        {
            _tokenService = tokenService;
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try 
            {
                if (request == null || string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest(new ResultError("Invalid login request."));
                }

                var authResponse = await _tokenService.LoginAsync(request, IpAddress);
                return Ok(new ResultSuccess(authResponse));
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                AOLLogger.Error($"Error processing login request: {ex.Message}", ex.Source, null, ex.StackTrace, IpAddress);
                return BadRequest(new ResultError($"Error processing login request: {ex.Message}"));
            }
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            try
            {
                var authResponse = await _tokenService.RefreshTokenAsync(refreshToken, IpAddress);
                return Ok(new ResultSuccess(authResponse));
            }
            catch (Exception ex)
            {
                
                // Log the exception (optional)
                AOLLogger.Error($"Error processing refresh token: {ex.Message}", ex.Source, null, ex.StackTrace, IpAddress);
                return BadRequest(new ResultError($"Error processing refresh token: {ex.Message}"));
            }
        }
        [AllowAnonymous]
        [HttpPatch("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Token) || string.IsNullOrEmpty(request.NewPassword))
                return BadRequest(new ResultError("Invalid reset password request."));
            if (request.NewPassword != request.ConfirmPassword)
                return BadRequest(new ResultError("New password and confirm password do not match."));
            try
            {
                var result = await _tokenService.GetUserTokensAsync(request.Token);
                if (result == null || !result.AccessTokenIsValid)
                {
                    return BadRequest(new ResultError("Reset password timeout."));
                }
                var resetResult = await _userService.ResetPassword(request.Email, request);
                if (!resetResult)
                    return BadRequest(new ResultError("Failed to reset password. Invalid token or email."));
                return Ok(new ResultSuccess("Password reset successfully."));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error resetting password: {ex.Message}");
                return BadRequest(new ResultError("Failed to reset password."));
            }
        }
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await _tokenService.LogoutAsync(UserId, IpAddress);
            return Ok(new ResultSuccess());
        }
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            var claims = User.Claims.ToList();

            var userInfo = new UserModel()
            {
                Id = new Guid(User.Identity.GetUserId()),
                Username = User.Identity?.Name,
                AvatarUrl = claims.FirstOrDefault(c => c.Type == "AvatarUrl")?.Value,
                Status = Enum.TryParse<UserStatus>(claims.FirstOrDefault(c => c.Type == "Status")?.Value, out var status)
                 ? status.ToString()
                 : UserStatus.Inactive.ToString(), // hoặc Default/Fallback,
                Email = claims.FirstOrDefault(c => c.Type == "Email")?.Value,
                Roles = claims.FirstOrDefault(c => c.Type == "Roles")?.Value?.Split(',').ToList(),
                CreatedAt = DateTime.TryParse(claims.FirstOrDefault(c => c.Type == "CreatedAt")?.Value, out var createdAt) ? createdAt : DateTime.UtcNow
            };
            return Ok(new ResultSuccess(userInfo));
        }

    }
}
