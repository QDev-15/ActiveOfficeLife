using ActiveOfficeLife.Application.Models;
using ActiveOfficeLife.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.ExtensitionModel
{
    public static class ExtensitionModel
    {
        public static UserModel ReturnModel(this User user)
        {
            return new UserModel()
            {
                Id = user.Id,
                Username = user.Username,
                AvatarUrl = user.AvatarUrl,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                RefreshTokenExpiryTime = user.RefreshTokenExpiryTime,
                Roles = user.Roles.Select(a => a.Name).ToList(),
                Token = user.Token
            };
        }
    }
}
