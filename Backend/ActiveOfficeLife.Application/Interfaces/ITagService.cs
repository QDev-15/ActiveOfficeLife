using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Common.Requests;

namespace ActiveOfficeLife.Application.Interfaces
{
    public interface ITagService
    {
        Task<List<TagModel>> GetAllTags();
        Task<(List<TagModel> Items, int Count)> Search(PagingTagRequest request);
        Task<TagModel> Add(TagModel tagModel);
        Task<TagModel> Update(TagModel tagModel);
        Task<TagModel> GetById(string id);
        Task<TagModel> GetByName(string name);
        Task<bool> IsExist(string name);
        Task<bool> Delete(string name);
    }
}
