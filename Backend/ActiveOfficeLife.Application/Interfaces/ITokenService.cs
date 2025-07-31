using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Common.Requests;
using ActiveOfficeLife.Common.Responses;

namespace ActiveOfficeLife.Application.Interfaces
{
    public interface ITokenService
    {
        Task<AuthResponse> CreateAsync(UserModel userModel, string ipAddress);
        Task<AuthResponse> LoginAsync(LoginRequest loginRequest, string ipAddress);
        Task<AuthResponse> RefreshTokenAsync(string refreshToken, string ipAddress);
        Task<List<UserTokenModel>> GetUserTokensAsync(Guid userId);
        Task<UserTokenModel> GetUserTokensAsync(string token);
        Task LogoutAsync(Guid userId, string ipAddress);
        string GenerateAccessToken(UserModel user);
        Task<string> GeneratePasswordResetTokenAsync();
        string GenerateRefreshToken(int daysValid);
    }
}
