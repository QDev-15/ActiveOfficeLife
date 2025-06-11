using ActiveOfficeLife.Application.Models;
using ActiveOfficeLife.Application.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserModel> GetUser(Guid id);
        Task<UserModel> Login(LoginRequest loginRequest);
        Task<UserModel> Register(RegisterRequest registerRequest);
        Task<UserModel> Update(UserModel model);
    }
}
