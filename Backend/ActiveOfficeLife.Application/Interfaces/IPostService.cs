using ActiveOfficeLife.Application.Models;
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
        Task<List<PostModel>> GetByKey(string keyWord);
        Task<List<PostModel>> Search(Requespa);
        Task<PostModel> Create(PostModel post);
        Task<PostModel> Update(PostModel post);
        Task<bool> Delete(Guid id);
    }
}
