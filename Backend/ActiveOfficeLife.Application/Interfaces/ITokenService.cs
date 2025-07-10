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
        Task<UserTokenModel> CreateAsync(string userId, string ipAddress);
        Task<AuthResponse> LoginAsync(LoginRequest loginRequest);
        Task<UserTokenModel> RefreshTokenAsync(string refreshToken, string ipAddress);
        string GenerateAccessToken(UserModel user);
        string GenerateRefreshToken(int daysValid);
        ClaimsPrincipal? ValidateToken(string token);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
