using ActiveOfficeLife.Application.ExtensitionModel;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Services
{
    public class SeoMetaDataService : ISeoMetaDataService
    {
        private readonly string serviceName = "";
        private readonly string className = "";
        private readonly ISeoMetadataRepository _seoMetadataRepository;
        private readonly IUnitOfWork _unitOfWork;
        public SeoMetaDataService(ISeoMetadataRepository seoMetadataRepository, IUnitOfWork unitOfWork) {
            _seoMetadataRepository = seoMetadataRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<SeoMetadataModel> Add(SeoMetadataModel seoMetadataModel)
        {
            try 
            {
                var seoMetadata = new SeoMetadata
                {
                    Id = Guid.NewGuid(),
                    MetaTitle = seoMetadataModel.MetaTitle,
                    MetaDescription = seoMetadataModel.MetaDescription,
                    MetaKeywords = seoMetadataModel.MetaKeywords,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _seoMetadataRepository.AddAsync(seoMetadata);
                await _unitOfWork.SaveChangesAsync();
                return seoMetadata.ReturnModel();
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                throw new Exception($"Error in {className}.{serviceName}.Add: {ex.Message}", ex);
            }
        }

        public async Task<bool> Delete(Guid id)
        {
            try 
            {
                var seoMetadata = await _seoMetadataRepository.GetByIdAsync(id);
                if (seoMetadata == null)
                {
                    // Log the error (not implemented here)
                    return false;
                }
                _seoMetadataRepository.Remove(seoMetadata);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                throw new Exception($"Error in {className}.{serviceName}.Delete: {ex.Message}", ex);
            }
        }

        public async Task<SeoMetadataModel> GetById(Guid id)
        {
            try 
            {
                var seoMetadata = await _seoMetadataRepository.GetByIdAsync(id);
                if (seoMetadata == null)
                {
                    // Log the error (not implemented here)
                    return null;
                }
                return seoMetadata.ReturnModel();
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                throw new Exception($"Error in {className}.{serviceName}.GetById: {ex.Message}", ex);
            }
        }

        public async Task<SeoMetadataModel> Update(SeoMetadataModel seoMetadataModel)
        {
            try
            {
                var seoMetadata = await _seoMetadataRepository.GetByIdAsync(seoMetadataModel.Id);
                if (seoMetadata == null)
                {
                    // Log the error (not implemented here)
                    throw new Exception("SEO Metadata not found.");
                }
                seoMetadata.MetaTitle = seoMetadataModel.MetaTitle;
                seoMetadata.MetaDescription = seoMetadataModel.MetaDescription;
                seoMetadata.MetaKeywords = seoMetadataModel.MetaKeywords;
                seoMetadata.UpdatedAt = DateTime.UtcNow;
                _seoMetadataRepository.Update(seoMetadata);
                await _unitOfWork.SaveChangesAsync();
                return seoMetadata.ReturnModel();
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                throw new Exception($"Error in {className}.{serviceName}.Update: {ex.Message}", ex);
            }
        }
    }
}
