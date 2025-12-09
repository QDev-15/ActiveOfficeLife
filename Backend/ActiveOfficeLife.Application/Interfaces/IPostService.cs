using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Common.Requests;
using ActiveOfficeLife.Common.Responses;

namespace ActiveOfficeLife.Application.Interfaces
{
    public interface IPostService
    {
        Task<(List<PostModel> Items, int count)> GetAll(PagingPostRequest? request);
        Task<(List<PostModel> Items, int count)> GetList(PagingPostRequest? request);
        Task<HotNewsResponse> GetHotNews();
        Task<PostModel> GetById(Guid id);
        Task<PostModel> GetByAlias(string slug);
        Task<List<PostModel>> GetByCategoryId(Guid categoryId, PagingPostRequest? request);
        Task<List<PostModel>> GetByKey(string keyWord);
        Task<List<PostModel>> Search(PagingPostRequest request);
        Task<PostModel> Create(PostModel post);
        Task<PostModel> Update(PostModel post);
        Task<bool> Delete(Guid id);
    }
}
