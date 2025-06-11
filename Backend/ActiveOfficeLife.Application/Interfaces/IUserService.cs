using ActiveOfficeLife.Application.Models;
using ActiveOfficeLife.Application.Models.Requests;
using ActiveOfficeLife.Application.Models.Responses;

namespace ActiveOfficeLife.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserModel> GetUser(Guid id);
        Task<UserModel> GetByRefreshToken(string refreshToken);
        Task<UserModel> GetByToken(string token);
        Task<UserModel> Create(RegisterRequest registerRequest);
        Task<UserModel> Update(UserModel model);
        Task<UserModel> Delete(Guid id);
    }
}
