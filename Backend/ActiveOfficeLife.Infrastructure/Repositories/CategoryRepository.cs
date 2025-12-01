using ActiveOfficeLife.Common.Requests;
using ActiveOfficeLife.Domain.EFCore.DBContext;
using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Infrastructure.Repositories
{
    public class CategoryRepository : _Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ActiveOfficeLifeDbContext context) : base(context)
        {

        }

        public async Task<bool> CheckExitsByName(string name, Guid id)
        {
            bool exists = await _context.Categories.AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.Id != id && x.IsDeleted == false);
            return exists;
        }

        public async Task<bool> CheckExitsBySlug(string slug, Guid id)
        {
            bool exists = await _context.Categories.AnyAsync(x => x.Slug.ToLower() == slug.ToLower() && x.Id != id && x.IsDeleted == false);
            return exists;
        }

        public async Task<bool> CheckUsing(Guid id)
        {
            // check xem Category này có được sử dụng trong các bài viết, hay có con được sử dụng trong các bài viết khác không
            // nếu có thì trả về true, ngược lại trả về false
            var category  = await _context.Categories
                .Include(x => x.Posts)
                .Include(x => x.Children)
                .FirstOrDefaultAsync(x => x.Id == id);
                        
            bool exists = CheckCategoryExistsPosts(category);
            return exists;
        }
        private bool CheckCategoryExistsPosts(Category? category)
        {
            if (category == null)
            {
                return false;
            }
            bool exists = false;
            // Kiểm tra xem Category có bài viết nào không
            if (category.Posts != null && category.Posts.Any())
            {
                exists = true;
            }
            // Kiểm tra xem Category có con nào có bài viết không
            foreach (var child in category.Children)
            {
                if (CheckCategoryExistsPosts(child))
                {
                    exists = true;
                }
            }
            return exists;
        }
        public new async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.Where(x => x.IsDeleted == false).Include(x => x.SeoMetadata).Include(x => x.Children).Include(x=> x.Parent).ToListAsync();
        }
        public async Task<(IEnumerable<Category> Categories, int Count)> GetAllWithPaging(PagingCategoryRequest request)
        {
            var allowedFields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "name", "Name" },
                { "slug", "Slug" },
                { "isActive", "IsActive" },
                { "parent", "parent" },
                
            };
            if (!allowedFields.TryGetValue(request.SortField ?? "", out var actualSortField))
            {
                actualSortField = "Name"; // fallback nếu không đúng
                request.SortField = actualSortField;
            }
            IQueryable<Category> query = _context.Categories.Where(x => x.IsDeleted == false);
            

            // check and filter by search text igonre case sensitivity
            if (!string.IsNullOrEmpty(request.SearchText))
            {
                var search = $"%{request.SearchText.ToLower()}%";
                query = query
                    .Where(c => (c.Parent != null && EF.Functions.Like(c.Parent.Name.ToLower(), search)) || EF.Functions.Like(c.Name.ToLower(), search) || EF.Functions.Like(c.Slug.ToLower(), search));
            }

            if (actualSortField == "parent")
            {
                //sort category first with parent null, and group by parent category
                if (request.SortDirection.ToLower() == "asc")
                {
                    query = query.OrderBy(c => c.ParentId == null ? 0 : 1).ThenBy(c => c.Parent.Name).ThenBy(c => c.Name);
                } else
                {
                    query = query.OrderByDescending(c => c.ParentId == null ? 0 : 1).ThenBy(c => c.Parent.Name).ThenBy(c => c.Name);
                }
                    
            } else if (!string.IsNullOrEmpty(actualSortField))
            {
                if (request.SortDirection.ToLower() == "asc")
                {
                    query = query.OrderBy(x => EF.Property<object>(x, actualSortField));
                }
                else
                {
                    query = query.OrderByDescending(x => EF.Property<object>(x, actualSortField));
                }
            }

            int count = await query.CountAsync();
            var categories = await query.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).Include(x => x.Parent).Include(x => x.SeoMetadata).ToListAsync();
            return (categories, count);
        }

        public async Task<Category?> GetById(Guid id)
        {
            return await _context.Categories.Include(x => x.SeoMetadata).Include(x => x.Parent).Include(x => x.Children).FirstOrDefaultAsync(x => x.Id == id);
        }
        // override Remove method to set IsDeleted = true instead of removing from database
        public new void Remove(Category entity)
        {
            if (entity.SeoMetadata != null)
            {
                _context.SeoMetadata.Remove(entity.SeoMetadata);
            }
            if (entity.Children != null && entity.Children.Any())
            {
                // remove all children categories
                foreach (var child in entity.Children)
                {
                    Remove(child);
                }
            }
            // set IsDeleted = true instead of removing from database
            _context.Categories.Remove(entity);
        }
        

        public async Task<IEnumerable<Category>> GetByParrentId(Guid parentId)
        {
            return await _context.Categories.Where(x => x.ParentId == parentId).Include(x => x.SeoMetadata).Include(x => x.Children).ToListAsync();
        }

        public async Task<Category> GetDefaultCategoryAsync()
        {
            var defaultCategory = await _context.Categories.FirstOrDefaultAsync();
            if (defaultCategory == null)
            {
                defaultCategory = new Category
                {
                    Name = "Default Category",
                    Slug = "default-category",
                    IsActive = true,
                    IsDeleted = false   
                };
                await _context.Categories.AddAsync(defaultCategory);
                await _context.SaveChangesAsync();
            }
            return defaultCategory;
        }

        public async Task<IEnumerable<Category>> GetByCategoryType(Guid typeId)
        {
            var categories = await _context.Categories
                .Where(c => c.CategoryTypeId == typeId && c.IsDeleted == false)
                .Include(c => c.SeoMetadata)
                .Include(c => c.Children)
                .ToListAsync();
            return categories;
        }
    }
}
