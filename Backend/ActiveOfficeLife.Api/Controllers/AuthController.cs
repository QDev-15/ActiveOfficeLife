using ActiveOfficeLife.Application.Common;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Application.Models;
using ActiveOfficeLife.Application.Models.Requests;
using ActiveOfficeLife.Application.Models.Responses;
using ActiveOfficeLife.Common.Enums;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Api.Controllers
{
    public class AuthController : BaseController
    {
        private readonly ITokenService _tokenService;
        public AuthController(ITokenService tokenService)
        {
            _tokenService = tokenService;
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
                 ? status
                 : UserStatus.Inactive, // hoặc Default/Fallback,
                Email = claims.FirstOrDefault(c => c.Type == "Email")?.Value,
                Roles = claims.FirstOrDefault(c => c.Type == "Roles")?.Value?.Split(',').ToList(),
                CreatedAt = DateTime.TryParse(claims.FirstOrDefault(c => c.Type == "CreatedAt")?.Value, out var createdAt) ? createdAt : DateTime.UtcNow
            };
            return Ok(new ResultSuccess(userInfo));
        }

    }
}
