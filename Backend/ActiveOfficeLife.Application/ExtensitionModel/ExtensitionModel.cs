using ActiveOfficeLife.Application.Models;
using ActiveOfficeLife.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.ExtensitionModel
{
    public static class ExtensitionModel
    {
        public static PostModel ReturnModel(this Post value)
        {
            return new PostModel()
            {
                Id = value.Id,
                Author = value.Author.ReturnModel(),
                AuthorId = value.AuthorId,
                Category = value.Category.ReturnModel(),
                CategoryId = value.CategoryId,
                Comments = value.Comments.Select(x => x.ReturnModel()).ToList(),
                Content = value.Content,
                CreatedAt = value.CreatedAt,
                SeoMetadata = value.SeoMetadata?.ReturnModel(),
                SeoMetadataId = value.SeoMetadataId,
                Slug = value.Slug,
                Status = value.Status,
                Summary = value.Summary,
                Tags = value.Tags?.Select(x => x.ReturnModel()).ToList(),
                Title = value.Title,
                UpdatedAt = value.UpdatedAt
            };
        }
        public static CommentModel ReturnModel(this Comment comment)
        {
            return new CommentModel()
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                ParentComment = comment.ParentComment != null ? new CommentModel()
                {
                    Id = comment.ParentComment.Id,
                    Content = comment.ParentComment.Content,
                    CreatedAt = comment.ParentComment.CreatedAt,
                    Post = new PostModel() { Id = comment.ParentComment.Post.Id, Content = comment.ParentComment.Post.Content, Title = comment.ParentComment.Post.Title },
                    UserId = comment.ParentComment.UserId,
                    User = new UserModel()
                    {
                        Id = comment.ParentComment.User.Id,
                        Email = comment.ParentComment.User.Email,
                        AvatarUrl = comment.ParentComment.User.AvatarUrl,
                        Roles = comment.ParentComment.User.Roles.Select(x => x.Name).ToList(),
                        Status = comment.ParentComment.User.Status
                    },
                    PostId = comment.ParentComment.PostId
                } : null,
                Post = new PostModel() { Id = comment.Post.Id, Content = comment.Post.Content, Title = comment.Post.Title },
                UserId = comment.UserId,
                User = new UserModel()
                {
                    Id = comment.User.Id,
                    Email = comment.User.Email,
                    AvatarUrl = comment.User.AvatarUrl,
                    Roles = comment.User.Roles.Select(x => x.Name).ToList(),
                    Status = comment.User.Status
                },
                PostId = comment.PostId,
                ParentCommentId = comment.ParentCommentId,
                Replies = comment.Replies.Select(x => x.ReturnModel()).ToList()
            };
        }
        public static TagModel ReturnModel(this Tag tag)
        {
            return new TagModel()
            {
                Id = tag.Id,
                Name = tag.Name,
                SeoMetadata = tag.SeoMetadata?.ReturnModel(),
                SeoMetadataId = tag.SeoMetadataId,
                Slug = tag.Slug
            };
        }
        public static RoleModel ReturnModel(this Role role)
        {
            return new RoleModel()
            {
               Id = role.Id,
               Name = role.Name,
               Description = role.Description,
               Users = role.Users != null ? role.Users.Select(x => x.ReturnModel()).ToList() : new List<UserModel>()
            };
        }
        public static UserModel ReturnModel(this User user)
        {
            return new UserModel()
            {
                Id = user.Id,
                Username = user.Username,
                AvatarUrl = user.AvatarUrl,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                RefreshToken = user.RefreshToken,
                Roles = user.Roles.Select(a => a.Name).ToList(),
                Token = user.Token
            };
        }
        public static CategoryModel ReturnModel(this Category category, bool isParrent = false)
        {
            if (category == null)
            {
                return null;
            }
            if (isParrent)
            {
                return new CategoryModel()
                {
                    Id = category.Id,
                    Name = category.Name,
                    Slug = category.Slug,
                    Description = category.Description,
                    SeoMetadataId = category.SeoMetadataId,
                    SeoMetadata = category.SeoMetadata?.ReturnModel(),
                    Children = []
                };
            }
            return new CategoryModel()
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                Description = category.Description,
                SeoMetadataId = category.SeoMetadataId,
                SeoMetadata = category.SeoMetadata?.ReturnModel(),
                ParentId = category.ParentId,
                Parent = category.Parent?.ReturnModel(true),
                Children = category.Children.Any() ? category.Children.Select(c => c.ReturnModel()).ToList() : [],
            };
        }
        public static SeoMetadataModel ReturnModel(this SeoMetadata seo)
        {
            return new SeoMetadataModel()
            {
                Id = seo.Id,
                MetaTitle = seo.MetaTitle,
                MetaDescription = seo.MetaDescription,
                MetaKeywords = seo.MetaKeywords,
                CreatedAt = seo.CreatedAt,
                UpdatedAt = seo.UpdatedAt
            };
        }
    }
}
