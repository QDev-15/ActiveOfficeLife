using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Domain.EFCore.DBContext;
using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
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

        


        public async Task<Setting> GetSettingById(Guid Id)
        {
            var setting = await _context.Settings.FindAsync(Id);
            return setting ?? throw new Exception("Setting not found");
        }

        public async Task<Setting> GetSettingByName(string name)
        {
            var setting = await _context.Settings.FirstOrDefaultAsync(s => s.Name == name);
            return setting ?? throw new Exception("Setting not found");
        }

        public async Task<Setting> GetSettingDefault()
        {
            return await _context.Settings.FirstOrDefaultAsync() ?? throw new Exception("Default setting not found");
        }

        public async Task<bool> IsExistsSettingName(Guid Id, string name)
        {
            bool isUnique = await _context.Settings.AnyAsync(s => s.Name.ToLower() == name.ToLower() && s.Id != Id);
            return isUnique;
        }

        

    }
}
