using ActiveOfficeLife.Common.Enums;

namespace ActiveOfficeLife.Common.Models
{
    public class PostModel
    {
        public Guid? Id { get; set; }

        public string? Title { get; set; } = null!;
        public string? Slug { get; set; } = null!;
        public string? Content { get; set; } = null!;
        public string? Summary { get; set; }
        public bool? IsFeaturedHome { get; set; }      // Tin nổi bật trang chủ
        public bool? IsHot { get; set; }               // Tin hot
        public bool? IsCenterHighlight { get; set; }   // Bài tiêu biểu block trung tâm
        public int? DisplayOrder { get; set; }        // Để sort trong mỗi nhóm

        public Guid? AuthorId { get; set; }
        public UserModel? Author { get; set; } = null!;

        public Guid? CategoryId { get; set; }
        public CategoryModel? Category { get; set; } = null!;

        public string? Status { get; set; } = "Draft";

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public List<Guid> TagIds { get; set; } = new();
        public List<CommentModel> Comments { get; set; } = new List<CommentModel>();
        public List<TagModel> Tags { get; set; } = new List<TagModel>();

        public Guid? SeoMetadataId { get; set; }
        public SeoMetadataModel? SeoMetadata { get; set; }
    }
}
