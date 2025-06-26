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
    public class TagRepository : _Repository<Tag>, ITagRepository
    {
        public TagRepository(ActiveOfficeLifeDbContext context) : base(context)
        {
        }

        public async Task<Tag?> GetByNameAsync(string name)
        {
            var tag = await _context.Tags.Where(t => t.Name == name.ToLower()).FirstOrDefaultAsync();
            if (tag == null)
            {
                return null;
            }
            return tag;
        }
    }
}
