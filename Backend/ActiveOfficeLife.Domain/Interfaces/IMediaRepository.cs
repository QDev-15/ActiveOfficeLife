using ActiveOfficeLife.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Domain.Interfaces
{
    public interface IMediaRepository : _IRepository<Media>
    {
        Task<List<Media>> GetMediaByFileId(string fileId);
    }
}
