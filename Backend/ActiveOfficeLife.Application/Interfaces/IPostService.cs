using ActiveOfficeLife.Application.Models;
using ActiveOfficeLife.Application.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Interfaces
{
    public interface IPostService
    {
        Task<PostModel> GetById(Guid id);
        Task<PostModel> GetByAlias(string slug);
        Task<List<PostModel>> GetByCategoryId(Guid categoryId, PagingRequest? request);
        Task<List<PostModel>> GetByKey(string keyWord);
        Task<List<PostModel>> Search(PagingRequest request);
        Task<PostModel> Create(PostModel post);
        Task<PostModel> Update(PostModel post);
        Task<bool> Delete(Guid id);
    }
}
