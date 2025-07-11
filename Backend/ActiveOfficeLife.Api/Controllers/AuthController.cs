using ActiveOfficeLife.Application.Common;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Application.Models;
using ActiveOfficeLife.Application.Models.Requests;
using ActiveOfficeLife.Common.Enums;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
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
                    return BadRequest("Invalid login request.");
                }
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                if (!string.IsNullOrEmpty(ipAddress))
                {
                    request.ipAddress = ipAddress;
                }
                else
                {
                    request.ipAddress = "Unknown IP";
                }
                var authResponse = await _tokenService.LoginAsync(request);
                return Ok(authResponse);
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                AOLLogger.Error($"Error processing login request: {ex.Message}", ex.Source, null, ex.StackTrace, request.ipAddress);
                return BadRequest($"Error processing login request: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            try
            {
                var authResponse = await _tokenService.RefreshTokenAsync(refreshToken, ipAddress);
                return Ok(authResponse);
            }
            catch (Exception ex)
            {
                
                // Log the exception (optional)
                AOLLogger.Error($"Error processing refresh token: {ex.Message}", ex.Source, null, ex.StackTrace, ipAddress);
                return BadRequest($"Error processing refresh token: {ex.Message}");
            }
        }
        [Authorize]
        [HttpGet("protected")]
        public IActionResult GetSecureData()
        {
            var username = User.Identity?.Name;
            return Ok(new { message = $"Hello {username}, this is protected data!" });
        }
        [Authorize]
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
            return Ok(userInfo);
        }

    }
}
