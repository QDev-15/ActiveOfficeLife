using ActiveOfficeLife.Domain.EFCore.DBContext;
using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Infrastructure.Repositories
{
    public class RoleRepository : _Repository<Role>, IRoleRepository
    {
        public RoleRepository(ActiveOfficeLifeDbContext context) : base(context)
        {
        }
    }
}
