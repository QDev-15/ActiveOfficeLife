using ActiveOfficeLife.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Models
{
    public class CategoryModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? Description { get; set; }

        // SEO
        public Guid? SeoMetadataId { get; set; }
        public SeoMetadataModel? SeoMetadata { get; set; }


        public Guid? ParentId { get; set; }                   // FK đến Category.Id (có thể null nếu là cha)
        public CategoryModel? Parent { get; set; }                 // Category cha
        public ICollection<CategoryModel> Children { get; set; } = new List<CategoryModel>(); // Các chuyên mục con
    }
}
