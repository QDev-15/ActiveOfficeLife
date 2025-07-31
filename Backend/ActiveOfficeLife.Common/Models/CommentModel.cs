
namespace ActiveOfficeLife.Common.Models
{
    public class CommentModel
    {
        public Guid? Id { get; set; }
        public Guid? PostId { get; set; }
        public Guid? UserId { get; set; }
        public string? Content { get; set; } = null!;
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public PostModel? Post { get; set; } = null!;
        public UserModel? User { get; set; } = null!;

        // Hỗ trợ phản hồi lồng nhau (self-referencing)
        public Guid? ParentCommentId { get; set; }
        public CommentModel? ParentComment { get; set; }
        public List<CommentModel> Replies { get; set; } = new List<CommentModel>();
    }
}
