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
    public class CategoryRepository : _Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ActiveOfficeLifeDbContext context) : base(context)
        {

        }
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.Where(x => x.IsDeleted == false).Include(x => x.SeoMetadata).ToListAsync();
        }
        public async Task<Category?> GetByIdAsync(Guid guidId)
        {
            return await _context.Categories.Include(x => x.SeoMetadata).FirstOrDefaultAsync(x => x.Id == guidId);
        }

        public async Task<IEnumerable<Category>> GetByParrentId(Guid parentId)
        {
            return await _context.Categories.Where(x => x.ParentId == parentId).Include(x => x.SeoMetadata).ToListAsync();
        }
    }
}
