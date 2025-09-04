using ActiveOfficeLife.Application.Common;
using ActiveOfficeLife.Application.ExtensitionModel;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Domain.Interfaces;

namespace ActiveOfficeLife.Application.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;
        private readonly _IUnitOfWork _unitOfWork;

        public TagService(ITagRepository tagRepository, _IUnitOfWork unitOfWork)
        {
            _tagRepository = tagRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<TagModel> Add(TagModel tagModel)
        {
            try
            {
                var existingTag = await this.IsExist(tagModel.Name);
                if (existingTag)
                {
                    AOLLogger.Error("Tag already exists.", "TagService", null);
                    // Log the error (not implemented here)
                    throw new Exception("Tag already exists.");
                }
                var tag = new Tag
                {
                    Id = Guid.NewGuid(),
                    Name = tagModel.Name,
                    Slug = tagModel.Slug,
                };
                if (tagModel.SeoMetadata != null)
                {
                    tag.SeoMetadata = new SeoMetadata
                    {
                        Id = tagModel.SeoMetadata?.Id ?? Guid.NewGuid(),
                        MetaDescription = tagModel.SeoMetadata?.MetaDescription,
                        MetaKeywords = tagModel.SeoMetadata?.MetaKeywords,
                        MetaTitle = tagModel.SeoMetadata?.MetaTitle,
                        CreatedAt = tagModel.SeoMetadata?.CreatedAt ?? DateTime.UtcNow,
                        UpdatedAt = tagModel.SeoMetadata?.UpdatedAt ?? DateTime.UtcNow
                    };
                }
                await _tagRepository.AddAsync(tag);
                await _unitOfWork.SaveChangesAsync();
                return tag.ReturnModel();
            }
            catch (Exception ex)
            {
                AOLLogger.Error(ex.Message, ex.Source, null, ex.StackTrace);
                // Log the exception (not implemented here)
                throw new Exception("Add tag error.");
            }
        }
        public async Task<bool> Delete(string name)
        {
            try
            {
                var tag = await _tagRepository.GetByNameAsync(name);
                if (tag == null)
                {
                    // Log the error (not implemented here)
                    return false;
                }
                _tagRepository.Remove(tag);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                AOLLogger.Error(ex.Message, ex.Source, null, ex.StackTrace);
                // Log the exception (not implemented here)
                throw new Exception("Delete tag error.");
            }
        }
        public async Task<TagModel> GetById(string id)
        {
            try
            {
                var tag = await _tagRepository.GetByIdAsync(Guid.Parse(id));
                if (tag == null)
                {
                    AOLLogger.Error("Tag not found.", "TagService", null);
                    // Log the error (not implemented here)
                    throw new Exception("Tag not found.");
                }
                return tag.ReturnModel();
            }
            catch (Exception ex)
            {
                AOLLogger.Error(ex.Message, ex.Source, null, ex.StackTrace);
                // Log the exception (not implemented here)
                throw new Exception("Get tag by id error.");
            }
        }
        public async Task<TagModel> GetByName(string name)
        {
            try
            {
                var tag = await _tagRepository.GetByNameAsync(name);
                if (tag == null)
                {
                    AOLLogger.Error("Tag not found.", "TagService", null);
                    // Log the error (not implemented here)
                    throw new Exception("Tag not found.");
                }
                return tag.ReturnModel();
            }
            catch (Exception ex)
            {
                AOLLogger.Error(ex.Message, ex.Source, null, ex.StackTrace);
                // Log the exception (not implemented here)
                throw new Exception("Get tag by id error.");
            }
        }

        public async Task<bool> IsExist(string name)
        {
            try
            {
                var tag = await _tagRepository.GetByNameAsync(name);
                if (tag == null)
                {
                    return false; // Tag does not exist
                }
                return true; // Tag exists
            }
            catch (Exception ex)
            {
                AOLLogger.Error(ex.Message, ex.Source, null, ex.StackTrace);
                // Log the exception (not implemented here)
                throw new Exception("Check if tag exists error.");
            }
        }
    }
}
