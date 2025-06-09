using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Services
{
    public class UserService : IUserService
    {
        Task<UserDto> IUserService.ValidateUserAsync(string userName, string password)
        {
            throw new NotImplementedException();
        }
    }
}
