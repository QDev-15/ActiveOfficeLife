using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Domain.Interfaces
{
    public interface ISettingRepository : _IRepository<Setting>
    {
        Task<bool> IsExistsSettingName(Guid Id, string name);
        Task<Setting> GetSettingById(Guid Id);
        Task<Setting> GetSettingByName(string name);
        Task<Setting> GetSettingDefault();

    }
}
