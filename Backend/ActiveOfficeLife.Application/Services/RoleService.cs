using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Application.Models;
using ActiveOfficeLife.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Services
{
    public class RoleService : IRoleService
    {
        public Task<RoleModel> Add(RoleModel role)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<RoleModel> Update(RoleModel role)
        {
            throw new NotImplementedException();
        }
    }
}
