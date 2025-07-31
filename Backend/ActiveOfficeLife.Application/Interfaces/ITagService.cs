using ActiveOfficeLife.Common.Models;

namespace ActiveOfficeLife.Application.Interfaces
{
    public interface ITagService
    {
        Task<TagModel> Add(TagModel tagModel);
        Task<TagModel> GetById(string id);
        Task<TagModel> GetByName(string name);
        Task<bool> IsExist(string name);
        Task<bool> Delete(string name);
    }
}
