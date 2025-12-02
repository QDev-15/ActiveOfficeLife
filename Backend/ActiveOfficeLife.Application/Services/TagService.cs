using ActiveOfficeLife.Application.Common;
using ActiveOfficeLife.Application.ExtensitionModel;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Common;
using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Common.Requests;
using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Domain.Interfaces;

namespace ActiveOfficeLife.Application.Services
{
    public class TagService : ITagService
    {
        private string serviceName = "";
        private readonly ITagRepository _tagRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TagService(ITagRepository tagRepository, IUnitOfWork unitOfWork)
        {
            _tagRepository = tagRepository;
            _unitOfWork = unitOfWork;
            serviceName = this.GetType().Name;
        }

        public async Task<TagModel> Add(TagModel tagModel)
        {
            try
            {
                // check if tagModel is null or tagModel is objet with empty name
                if (tagModel == null || string.IsNullOrWhiteSpace(tagModel.Name))
                {
                    tagModel = new TagModel()
                    {
                        Name = "New Tag" + Helper.GenerateRandomString(4),
                        SeoMetadata = tagModel?.SeoMetadata
                    };
                    tagModel.Slug = Helper.GenerateSlug(tagModel.Name);
                }
                var existingTag = tagModel == null ? false : await this.IsExist(tagModel.Name);
                if (existingTag)
                {
                    string message = "Tag already exists with name: " + tagModel.Name;
                    AOLLogger.Error(new LogProperties() {
                        Message = message
                    });
                    // Log the error (not implemented here)
                    throw new Exception("Tag already exists.");
                }
                var tag = new Tag
                {
                    Id = Guid.NewGuid(),
                    Name = tagModel!.Name ?? "new tag " + Helper.GenerateRandomString(4),
                    SeoMetadata = new SeoMetadata()
                    {
                        Id = Guid.NewGuid(),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };
                tag.Slug = string.IsNullOrWhiteSpace(tagModel.Slug) ? Helper.GenerateSlug(tag.Name) : tagModel.Slug;
                if (tagModel.SeoMetadata != null)
                {
                    tag.SeoMetadata.MetaTitle = tagModel.SeoMetadata.MetaTitle;
                    tag.SeoMetadata.MetaDescription = tagModel.SeoMetadata.MetaDescription;
                    tag.SeoMetadata.MetaKeywords = tagModel.SeoMetadata.MetaKeywords;
                }
                await _tagRepository.AddAsync(tag);
                await _unitOfWork.SaveChangesAsync();
                return tag.ReturnModel();
            }
            catch (Exception ex)
            {
                AOLLogger.Error(ex);
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
                AOLLogger.Error(ex);
                // Log the exception (not implemented here)
                throw new Exception("Delete tag error.");
            }
        }

        public async Task<List<TagModel>> GetAllTags()
        {
            var tags = await _tagRepository.GetAllAsync();
            return tags.Select(t => t.ReturnModel()).ToList();
        }

        public async Task<TagModel> GetById(string id)
        {
            try
            {
                var tag = await _tagRepository.GetByIdAsync(Guid.Parse(id));
                if (tag == null)
                {
                    string message = "Tag not found with id: " + id;
                    AOLLogger.Error(new LogProperties() { Message = message });
                    // Log the error (not implemented here)
                    throw new Exception("Tag not found.");
                }
                return tag.ReturnModel();
            }
            catch (Exception ex)
            {
                AOLLogger.Error(ex);
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
                    string message = "Tag not found with name: " + name;
                    AOLLogger.Error(new LogProperties() { Message = message });
                    // Log the error (not implemented here)
                    throw new Exception("Tag not found.");
                }
                return tag.ReturnModel();
            }
            catch (Exception ex)
            {
                AOLLogger.Error(ex);
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
                AOLLogger.Error(ex);
                // Log the exception (not implemented here)
                throw new Exception("Check if tag exists error.");
            }
        }

        public async Task<(List<TagModel> Items, int Count)> Search(PagingTagRequest request)
        {
            string msgHdr = serviceName + "-" + nameof(Search);
            try
            {
                if (request == null)
                {
                    AOLLogger.Error($"{msgHdr}: PagingRequest is null.");
                    throw new ArgumentNullException(nameof(request), "PagingRequest cannot be null.");
                }
                var tagResult = await _tagRepository.GetAllWithPaging(request);
                if (tagResult.Items == null || !tagResult.Items.Any())
                {
                    AOLLogger.Error($"{msgHdr}: No Tags found for keyword {request.SearchText}.");
                    return (new List<TagModel>(), 0); // No posts found
                }
                return (tagResult.Items.Select(p => p.ReturnModel()).ToList(), tagResult.Count);
            }
            catch (Exception ex)
            {
                string message = $"{serviceName}-{nameof(Search)}: Error searching tags. Exception: {ex.Message}";
                AOLLogger.Error(message, ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<TagModel> Update(TagModel tagModel)
        {
            string msgHdr = serviceName + "-" + nameof(Update);
            try
            {
                if (tagModel == null || tagModel.Id == Guid.Empty)
                {
                    AOLLogger.Error($"{msgHdr}: TagModel is null or Id is empty.");
                    throw new ArgumentNullException(nameof(tagModel), "TagModel cannot be null and Id must be provided.");
                }
                Guid.TryParse(tagModel.Id.ToString(), out Guid tagId);
                var existingTag = await _tagRepository.GetByIdAsync(tagId);
                if (existingTag == null)
                {
                    AOLLogger.Error($"{msgHdr}: Tag with Id {tagModel.Id} not found.");
                    throw new Exception("Tag not found.");
                }
                // Check for name change and uniqueness
                if (!string.Equals(existingTag.Name, tagModel.Name, StringComparison.OrdinalIgnoreCase))
                {
                    var nameExists = _tagRepository.GetByNameAsync(tagModel.Name).Result;
                    if (nameExists != null)
                    {
                        AOLLogger.Error($"{msgHdr}: Tag name {tagModel.Name} already exists.");
                        throw new Exception("Tag name already exists.");
                    }
                    existingTag.Name = tagModel.Name;
                }
                existingTag.Slug = tagModel.Slug;
                // Update SEO metadata
                if (tagModel.SeoMetadata != null)
                {
                    if (existingTag.SeoMetadata == null)
                    {
                        existingTag.SeoMetadata = new SeoMetadata
                        {
                            Id = Guid.NewGuid(),
                            CreatedAt = DateTime.UtcNow
                        };
                    }
                    existingTag.SeoMetadata.MetaTitle = tagModel.SeoMetadata.MetaTitle;
                    existingTag.SeoMetadata.MetaDescription = tagModel.SeoMetadata.MetaDescription;
                    existingTag.SeoMetadata.MetaKeywords = tagModel.SeoMetadata.MetaKeywords;
                    existingTag.SeoMetadata.UpdatedAt = DateTime.UtcNow;
                }
                _tagRepository.Update(existingTag);
                await _unitOfWork.SaveChangesAsync();
                return existingTag.ReturnModel();
            }
            catch (Exception ex)
            {
                string message = $"{msgHdr}: Exception occurred while updating tag. Exception: {ex.Message}";
                AOLLogger.Error(message, ex);
                throw new Exception("Update tag error.");
            }
        }
    }
}
