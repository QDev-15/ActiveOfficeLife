using ActiveOfficeLife.Application.Models;
using ActiveOfficeLife.Application.Models.Requests;
using ActiveOfficeLife.Application.Models.Responses;

namespace ActiveOfficeLife.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserModel> GetUser(Guid id);
        Task<List<UserModel>> GetAll(int page, int pageSize);
        Task<List<UserModel>> GetAll(int index, int pageSize, bool desc);
        Task<UserModel> GetByRefreshToken(string refreshToken);
        Task<UserModel> GetByToken(string token);
        Task<UserModel> Create(RegisterRequest registerRequest);
        Task<UserModel> Update(UserModel model);
        Task<bool> Delete(Guid id);
        
    }
}
