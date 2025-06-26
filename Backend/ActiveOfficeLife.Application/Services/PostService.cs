using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Application.Models;
using ActiveOfficeLife.Application.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Services
{
    public class PostService : IPostService
    {
        public Task<PostModel> Create(PostModel post)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<PostModel> GetByAlias(string slug)
        {
            throw new NotImplementedException();
        }

        public Task<PostModel> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<PostModel>> GetByKey(string keyWord)
        {
            throw new NotImplementedException();
        }

        public Task<List<PostModel>> Search(PagingRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<PostModel> Update(PostModel post)
        {
            throw new NotImplementedException();
        }
    }
}
