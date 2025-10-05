using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Common.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Interfaces
{
    public interface IAdService
    {
        Task<AdModel> CreateAdAsync(AdModel adModel);
        Task<AdModel> UpdateAdAsync(AdModel admodel);
        Task<bool> DeleteAdAsync(Guid id);
        Task<(List<AdModel> Items, int Count)> GetAllAdsAsync(PagingAdRequest pagingAdRequest);
        Task<AdModel?> GetAdByIdAsync(Guid id);
    }
}
