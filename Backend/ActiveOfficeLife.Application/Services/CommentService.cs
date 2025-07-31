using ActiveOfficeLife.Application.Common;
using ActiveOfficeLife.Application.ExtensitionModel;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Domain.Interfaces;

namespace ActiveOfficeLife.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly _IUnitOfWork _iUnitOfWork;
        public CommentService(ICommentRepository commentRepository, _IUnitOfWork iUnitOfWork)
        {
            _commentRepository = commentRepository;
            _iUnitOfWork = iUnitOfWork;
        }
        public async Task<CommentModel> AddComment(string postId, string userId, string content)
        {
            try
            {
                var comment = new Comment()
                {
                    PostId = Guid.Parse(postId),
                    UserId = Guid.Parse(userId),
                    Content = content,
                    CreatedAt = DateTime.UtcNow
                };
                await _commentRepository.AddAsync(comment);
                await _iUnitOfWork.SaveChangesAsync();
                return comment.ReturnModel();
            }
            catch (Exception ex)
            {
                AOLLogger.Error(ex.Message, ex.Source, userId, ex.StackTrace);
                // Log the exception (not implemented here)
                throw new Exception("Add comment error.");
            }
        }

        public async Task<bool> Delete(string commentId, string userId)
        {
            try
            {
                var comment = await _commentRepository.GetByIdAsync(Guid.Parse(commentId));
                if (comment == null || comment.UserId != Guid.Parse(userId))
                {
                    AOLLogger.Error("Comment not found or user not authorized to delete.", "CommentService", userId);
                    return false;
                }
                _commentRepository.RemoveAsync(comment);
                await _iUnitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                AOLLogger.Error(ex.Message, ex.Source, userId, ex.StackTrace);
                // Log the exception (not implemented here)
                throw new NotImplementedException();
            }
        }
    }
}
