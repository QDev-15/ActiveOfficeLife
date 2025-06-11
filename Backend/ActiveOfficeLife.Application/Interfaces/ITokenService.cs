using ActiveOfficeLife.Application.Models;
using ActiveOfficeLife.Application.Models.Requests;
using ActiveOfficeLife.Application.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Interfaces
{
    public interface ITokenService
    {
        Task<AuthResponse> LoginAsync(LoginRequest loginRequest);
        Task<AuthResponse> RefreshTokenAsync(string refreshToken);
        string GenerateAccessToken(UserModel user);
        string GenerateRefreshToken(int daysValid);
        ClaimsPrincipal? ValidateToken(string token);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
