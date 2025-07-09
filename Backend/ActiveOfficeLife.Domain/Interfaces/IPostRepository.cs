using ActiveOfficeLife.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Domain.Interfaces
{
    public interface IPostRepository : _IRepository<Post>
    {
        Task<Post?> GetByAliasAsync(string slug);
        Task<List<Post>> GetByKeyAsync(string keyWord);
        Task<List<Post>> SearchAsync(string keyWord, int pageNumber, int pageSize);
        Task<List<Post>> GetPostsByCategoryAsync(Guid categoryId, int pageNumber, int pageSize);
        Task<List<Post>> GetPostsByAuthorAsync(Guid authorId, int pageNumber, int pageSize);
    }
}
