using ActiveOfficeLife.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
