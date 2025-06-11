using ActiveOfficeLife.Application.Common;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Application.Models;
using ActiveOfficeLife.Application.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ActiveOfficeLife.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public UserController(IUserService userService, ITokenService token)
        {
            _userService = userService;
            _tokenService = token;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Invalid login data." });
            }
            try
            {
                request.ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
                var authResponse = await _tokenService.LoginAsync(request);
                return Ok(authResponse);
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework)
                AOLLogger.Error($"Login failed for user {request.UserName}: {ex.Message}");
                return Unauthorized(new { message = "Invalid credentials." });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
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

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(string refreshToken)
        {
            try
            {
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return BadRequest(new { message = "Refresh token is required." });
                }
                var user = await _userService.GetByRefreshToken(refreshToken);
                return Ok(user);
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error refreshing token: {ex.Message}");
                return Unauthorized(new { message = "Invalid refresh token." });
            }
        }

        [HttpGet("validate")]
        public IActionResult ValidateToken([FromQuery] string token)
        {
            var principal = _tokenService.ValidateToken(token);
            return principal != null ? Ok("Valid") : Unauthorized();
        }
    }
}
