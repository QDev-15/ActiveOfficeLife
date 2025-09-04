using ActiveOfficeLife.Application.Common;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Application.Services;
using ActiveOfficeLife.Common.Enums;
using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Common.Requests;
using ActiveOfficeLife.Common.Responses;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using GoogleApi.Interfaces;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ActiveOfficeLife.Api.Controllers
{
    public class AuthController : BaseController
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private readonly CustomMemoryCache _cache;
        private readonly IGoogleDriveInterface _googleDriveInterface;
        private readonly ISettingService _settingService;
        private readonly AppConfigService _appConfigService;
        public AuthController(ISettingService settingService, ITokenService tokenService, IUserService userService,
            CustomMemoryCache cache, IGoogleDriveInterface googleDriveInterface, AppConfigService appConfigService)
        {
            _appConfigService = appConfigService ?? throw new ArgumentNullException(nameof(appConfigService));
            _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
            _googleDriveInterface = googleDriveInterface ?? throw new ArgumentNullException(nameof(googleDriveInterface));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _tokenService = tokenService;
            _userService = userService;
        }
        [AllowAnonymous]
        [HttpGet("callback")]
        public async Task<IActionResult> Callback(string code, string state, string orgId)
        {
            var parsed = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(Request.Query["state"].ToString());

            orgId = parsed.ContainsKey("orgId") ? parsed["orgId"].ToString() : null;
            state = parsed.ContainsKey("state") ? parsed["state"].ToString() : null;
            if (string.IsNullOrEmpty(code))
                return BadRequest("Missing code");
            if (string.IsNullOrEmpty(orgId))
                return BadRequest("Missing orgId");
            var setting = await _settingService.GetDefault(orgId);

            var token = await _googleDriveInterface.ExchangeCodeForTokenAsync(new ClientSecrets()
            {
                ClientId = setting.GoogleClientId,
                ClientSecret = setting.GoogleClientSecretId
            }, code, _appConfigService.AppConfigs.ApiUrl + "/api/auth/callback");
            setting.GoogleToken = JsonConvert.SerializeObject(token);
            await _settingService.Update(setting);

            return Ok("Google OAuth2 Login Success! Token saved.");
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest(new ResultError("Invalid login request.", "400"));
                }

                var authResponse = await _tokenService.LoginAsync(request, IpAddress);
                return Ok(new ResultSuccess(authResponse));
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                AOLLogger.Error($"Error processing login request: {ex.Message}", ex.Source, null, ex.StackTrace, IpAddress);
                return BadRequest(new ResultError($"Error processing login request: {ex.Message}", "400"));
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
                return BadRequest(new ResultError($"Error processing refresh token: {ex.Message}", "400"));
            }
        }
        [AllowAnonymous]
        [HttpPatch("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Token) || string.IsNullOrEmpty(request.NewPassword))
                return BadRequest(new ResultError("Invalid reset password request.", "400"));
            if (request.NewPassword != request.ConfirmPassword)
                return BadRequest(new ResultError("New password and confirm password do not match.", "400"));
            try
            {
                var result = await _tokenService.GetUserTokensAsync(request.Token);
                if (result == null || !result.AccessTokenIsValid)
                {
                    return BadRequest(new ResultError("Reset password timeout.", "400"));
                }
                var resetResult = await _userService.ResetPassword(request.Email, request);
                if (!resetResult)
                    return BadRequest(new ResultError("Failed to reset password. Invalid token or email.", "400"));
                return Ok(new ResultSuccess("Password reset successfully."));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error resetting password: {ex.Message}");
                return BadRequest(new ResultError("Failed to reset password.", "400"));
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

        [HttpGet("clearCache")]
        public IActionResult ClearCache()
        {
            _cache.Clear();
            return Ok(new ResultSuccess("Cache cleared successfully."));
        }

        
    }
}
