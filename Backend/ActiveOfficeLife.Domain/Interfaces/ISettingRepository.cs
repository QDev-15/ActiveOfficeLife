using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Infrastructure.EFCore.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Domain.Interfaces
{
    public interface ISettingRepository : _IRepository<Setting>
    {
    }
}
