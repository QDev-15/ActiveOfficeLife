using ActiveOfficeLife.Common.Models;

namespace ActiveOfficeLife.Application.Interfaces
{
    public interface ICommentService
    {
        Task<CommentModel> AddComment(string postId, string userId, string content);
        Task<bool> Delete(string commentId, string userId);
    }
}
