using ActiveOfficeLife.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Interfaces
{
    public interface ICommentService
    {
        Task<CommentModel> AddComment(string postId, string userId, string content);
        Task<bool> Delete(string commentId, string userId);
    }
}
