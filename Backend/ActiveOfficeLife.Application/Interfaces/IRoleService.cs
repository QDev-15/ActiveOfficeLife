using ActiveOfficeLife.Application.Models;
using ActiveOfficeLife.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Interfaces
{
    public interface IRoleService
    {
        Task<RoleModel> Add(RoleModel role);
        Task<RoleModel> Update(RoleModel role);
        Task<bool> Delete(Guid id);
    }
}
