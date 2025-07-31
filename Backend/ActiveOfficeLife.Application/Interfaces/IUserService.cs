using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Common.Requests;

namespace ActiveOfficeLife.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserModel> GetUser(Guid id);
        Task<UserModel> GetByUsername(string username);
        Task<UserModel> GetByEmail(string email);
        Task<UserModel> GetByPhoneNumber(string phone);
        Task<List<UserModel>> GetAll(int page, int pageSize);
        Task<List<UserModel>> GetAll(string searchText, int index, int pageSize, bool desc);
        Task<UserModel> GetByRefreshToken(string refreshToken);
        Task<UserModel> GetByToken(string token);
        Task<UserModel> Create(RegisterRequest registerRequest);
        Task<UserModel> Update(UserModel model);
        Task<UserModel> ForgotPassword(ForgotPasswordRequest forgotPasswordRequest);
        Task<bool> ChangePassword(Guid id, ChangePasswordRequest changePasswordRequest);
        Task<bool> ResetPassword(string email, ResetPasswordRequest changePasswordRequest);
        Task<bool> Delete(Guid id);
        
    }
}
