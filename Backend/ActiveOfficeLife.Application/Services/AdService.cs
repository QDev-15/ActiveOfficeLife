using ActiveOfficeLife.Application.ExtensitionModel;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Common.Requests;
using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Services
{
    public class AdService : IAdService
    {
        private readonly IAdRepository _adRepository;
        private readonly IUnitOfWork _unitOfWork;
        public AdService(IAdRepository adRepository, IUnitOfWork unitOfWork)
        {
            _adRepository = adRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<AdModel> CreateAdAsync(AdModel adModel)
        {
            var adEntity = new Ad
            {
                Id = Guid.NewGuid(),
                Name = adModel.Name,
                Type = adModel.Type,
                ImageUrl = adModel.ImageUrl,
                Link = adModel.Link,
                StartDate = adModel.StartDate,
                EndDate = adModel.EndDate,
                Status = adModel.Status
            };
            await _adRepository.AddAsync(adEntity);
            await _unitOfWork.SaveChangesAsync();
            return adEntity.ReturnModel();
        }

        public async Task<bool> DeleteAdAsync(Guid id)
        {
            var ad = await _adRepository.GetByIdAsync(id);
            if (ad == null)
            {
                return false;
            }
            _adRepository.Remove(ad);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<AdModel?> GetAdByIdAsync(Guid id)
        {
            var ad = await _adRepository.GetByIdAsync(id);
            if (ad == null)
            {
                return null;
            }
            return ad.ReturnModel();
        }

        public async Task<(List<AdModel> Items, int Count)> GetAllAdsAsync(PagingAdRequest pagingAdRequest)
        {
            var (adEntities, count) = await _adRepository.GetAllWithPaging(pagingAdRequest);
            var adModels = adEntities.Select(ad => ad.ReturnModel()).ToList();
            return (adModels, count);
        }

        public async Task<AdModel> UpdateAdAsync(AdModel admodel)
        {
            var ad = await _adRepository.GetByIdAsync(admodel.Id);
            if (ad == null)
            {
                throw new Exception("Ad not found");
            }
            ad.Name = admodel.Name;
            ad.Type = admodel.Type;
            var imageUrl = admodel.ImageUrl?.Trim();
            ad.ImageUrl = string.IsNullOrEmpty(imageUrl) ? null : imageUrl;
            ad.Link = admodel.Link;
            ad.StartDate = admodel.StartDate;
            ad.EndDate = admodel.EndDate;
            ad.Status = admodel.Status;
            _adRepository.Update(ad);
            await _unitOfWork.SaveChangesAsync();
            return ad.ReturnModel();
        }
    }
}
