﻿using ActiveOfficeLife.Application.Common;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Application.Models;
using ActiveOfficeLife.Application.Models.Requests;
using Microsoft.AspNetCore.Authorization;
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

        [AllowAnonymous]
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

        [Authorize]
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
