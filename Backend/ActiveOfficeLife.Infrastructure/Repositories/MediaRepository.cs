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
    public class MediaRepository : _Repository<Media>, IMediaRepository
    {
        public MediaRepository(ActiveOfficeLifeDbContext context) : base(context)
        {
        }

        public async Task<List<Media>> GetMediaByFileId(string fileId)
        {
            return await _context.Media.Where(x => x.FileId == fileId).ToListAsync();
        }
    }
}
