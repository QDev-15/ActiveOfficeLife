using ActiveOfficeLife.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> ValidateUserAsync(string userName, string password);
    }
}
