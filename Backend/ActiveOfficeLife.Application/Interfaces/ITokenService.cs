using ActiveOfficeLife.Application.Models;
using ActiveOfficeLife.Application.Models.Requests;
using ActiveOfficeLife.Application.Models.Responses;
using ActiveOfficeLife.Domain.Entities;
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
        Task<AuthResponse> CreateAsync(UserModel userModel, string ipAddress);
        Task<AuthResponse> LoginAsync(LoginRequest loginRequest, string ipAddress);
        Task<AuthResponse> RefreshTokenAsync(string refreshToken, string ipAddress);
        Task<List<UserTokenModel>> GetUserTokensAsync(Guid userId);
        Task LogoutAsync(Guid userId, string ipAddress);
        string GenerateAccessToken(UserModel user);
        string GenerateRefreshToken(int daysValid);
    }
}
