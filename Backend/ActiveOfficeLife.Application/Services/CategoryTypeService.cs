using ActiveOfficeLife.Application.Common;
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
    public class CategoryTypeService : ICategoryTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryTypeRepository _categoryTypeRepository;
        public CategoryTypeService(IUnitOfWork unitOfWork, ICategoryTypeRepository categoryTypeRepository) {
            this._unitOfWork = unitOfWork;
            this._categoryTypeRepository = categoryTypeRepository;
        }
        public async Task<CategoryTypeModel?> Add(CategoryTypeModel model)
        {
            var categoryType = new CategoryType()
            {
                Name = model.Name
            };
            try
            {
                await _categoryTypeRepository.AddAsync(categoryType);
                await _unitOfWork.SaveChangesAsync();
                return categoryType.ReturnModel();
            }
            catch (Exception ex) {
                AOLLogger.Error(ex);
                throw (new Exception(ex.Message));
            }
        }

        public async Task<bool> Delete(Guid id)
        {
            try
            {
                var categoryType = await _categoryTypeRepository.GetById(id);
                _categoryTypeRepository.Remove(categoryType!);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) {
                AOLLogger.Error(ex);
                return false;
            }
        }

        public async Task<IEnumerable<CategoryTypeModel>> GetAll()
        {
            try
            {
                var categoryTypes = await _categoryTypeRepository.GetAll();
                return categoryTypes.Select(x => x.ReturnModel()).ToList();
            } catch(Exception ex)
            {
                AOLLogger.Error(ex);
                throw (new Exception(ex.Message));
            }
        }

        public async Task<CategoryTypeModel?> GetById(Guid id)
        {
            try
            {
                var categoryType = await _categoryTypeRepository.GetById(id);
                return categoryType?.ReturnModel();
            }
            catch (Exception ex)
            {
                AOLLogger.Error(ex);
                throw (new Exception(ex.Message));
            }
        }
        public async Task<CategoryTypeModel?> Update(CategoryTypeModel model)
        {
            try {                 
                var categoryType = await _categoryTypeRepository.GetById(model.Id);
                if (categoryType != null)
                {
                    categoryType.Name = model.Name;
                    _categoryTypeRepository.Update(categoryType);
                    _unitOfWork.SaveChangesAsync().Wait();
                    return (categoryType.ReturnModel());
                }
                return null;
            }
            catch (Exception ex)
            {
                AOLLogger.Error(ex);
                throw (new Exception(ex.Message));
            }
        }
    }
}
