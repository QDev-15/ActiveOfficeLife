using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Interfaces
{
    public interface ISettingService
    {
        Task<SettingModel> GetById(Guid id);
        Task<SettingModel> GetDefault(string? id);
        Task<SettingModel> Create(SettingModel setting);
        Task<SettingModel> Update(SettingModel updateModel);
    }
}
