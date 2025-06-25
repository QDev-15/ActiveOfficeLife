using ActiveOfficeLife.Common.Enums;

namespace ActiveOfficeLife.Application.Models
{
    public class PostModel
    {
        public Guid? Id { get; set; }

        public string? Title { get; set; } = null!;
        public string? Slug { get; set; } = null!;
        public string? Content { get; set; } = null!;
        public string? Summary { get; set; }

        public Guid? AuthorId { get; set; }
        public UserModel? Author { get; set; } = null!;

        public Guid? CategoryId { get; set; }
        public CategoryModel? Category { get; set; } = null!;

        public PostStatus? Status { get; set; } = PostStatus.Draft;

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public List<CommentModel> Comments { get; set; } = new List<CommentModel>();
        public List<TagModel> Tags { get; set; } = new List<TagModel>();

        public Guid? SeoMetadataId { get; set; }
        public SeoMetadataModel? SeoMetadata { get; set; }
    }
}
