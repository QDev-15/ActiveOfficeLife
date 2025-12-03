using ActiveOfficeLife.Common.Requests;
using ActiveOfficeLife.Domain.EFCore.DBContext;
using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Infrastructure.Repositories
{
    public class CategoryTypeRepository : _Repository<CategoryType>, ICategoryTypeRepository
    {
        public CategoryTypeRepository(ActiveOfficeLifeDbContext context) : base(context)
        {

        }


        public async Task<IEnumerable<CategoryType>> GetAll()
        {
            var cateTypes = await _context.CategoryTypes.Include(ct => ct.Categories).ToListAsync();
            return cateTypes;
        }

        public Task<CategoryType> GetById(Guid? id)
        {
            var cateType = _context.CategoryTypes.Include(ct => ct.Categories)
                .FirstOrDefaultAsync(ct => ct.Id == id);
            return cateType;
        }
    }
}
