using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? Description { get; set; }

        // SEO
        public Guid? SeoMetadataId { get; set; }
        public SeoMetadata? SeoMetadata { get; set; }


        public Guid? ParentId { get; set; }                   // FK đến Category.Id (có thể null nếu là cha)
        public Category? Parent { get; set; }                 // Category cha
        public ICollection<Category> Children { get; set; } = new List<Category>(); // Các chuyên mục con

        public ICollection<Post> Posts { get; set; } = new List<Post>();  // Bài viết thuộc chuyên mục
    }

}
