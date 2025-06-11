using ActiveOfficeLife.Domain.EFCore.DBContext;
using ActiveOfficeLife.Domain.Interfaces;
using ActiveOfficeLife.Infrastructure.EFCore.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Infrastructure.Repositories
{
    public class SettingRepository : _Repository<Setting>, ISettingRepository
    {
        public SettingRepository(ActiveOfficeLifeDbContext context) : base(context)
        {
        }
    }
}
