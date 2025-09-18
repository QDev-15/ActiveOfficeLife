using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Domain.Entities;
using Google.Apis.Auth.OAuth2.Responses;
using Newtonsoft.Json;

namespace ActiveOfficeLife.Application.ExtensitionModel
{
    public static class ExtensitionModel
    {
        public static TokenResponse ConvertToResponseToken(this string jsonToken)
        {
            var token = JsonConvert.DeserializeObject<TokenResponse>(jsonToken);
            return token;
        }
        public static string ConvertToJsonToken(this TokenResponse token)
        {
            var json = JsonConvert.SerializeObject(token);
            return json;
        }
        public static MediaModel ReturnModel(this Media media)
        {
            return new MediaModel()
            {
                Id = media.Id,
                FileId = media.FileId,
                FileName = media.FileName,
                FilePath = media.FilePath,
                FileType = media.FileType,
                FileSize = media.FileSize,
                MediaType = media.MediaType,
                UploadedAt = media.UploadedAt,
                UploadedByUserId = media.UploadedByUserId,
                UploadedBy = media.UploadedBy != null ? new UserModel()
                {
                    Id = media.UploadedBy.Id,
                    Email = media.UploadedBy.Email,
                    AvatarUrl = media.UploadedBy.AvatarUrl,
                    Roles = media.UploadedBy.Roles.Select(x => x.Name).ToList(),
                    Status = media.UploadedBy.Status.ToString()
                } : null
            };
        }
        public static UserTokenModel ReturnModel(this UserToken userToken)
        {
            return new UserTokenModel()
            {
                Id = userToken.Id,
                UserId = userToken.UserId,
                User = userToken.User.ReturnModel(),
                AccessToken = userToken.AccessToken,
                AccessTokenExpiresAt = userToken.AccessTokenExpiresAt,
                IpAddress = userToken.IpAddress,
                RefreshToken = userToken.RefreshToken,
                RefreshTokenExpiresAt = userToken.RefreshTokenExpiresAt,
                CreatedAt = userToken.CreatedAt
            };
        }
        public static PostModel ReturnModel(this Post value)
        {
            return new PostModel()
            {
                Id = value.Id,
                Author = value.Author == null ? new UserModel()
                {
                    Id = value.AuthorId
                } : value.Author.ReturnModel(),
                AuthorId = value.AuthorId,
                Category = value.Category == null ? new CategoryModel()
                {
                    Id = value.CategoryId
                } : value.Category.ReturnModel(),
                CategoryId = value.CategoryId,
                Comments = value.Comments.Select(x => x.ReturnModel()).ToList(),
                Content = value.Content,
                CreatedAt = value.CreatedAt,
                SeoMetadata = value.SeoMetadata?.ReturnModel(),
                SeoMetadataId = value.SeoMetadataId,
                Slug = value.Slug,
                Status = value.Status.ToString(),
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
                        Status = comment.ParentComment.User.Status.ToString()
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
                    Status = comment.User.Status.ToString()
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
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Status = user.Status.ToString(),
                CreatedAt = user.CreatedAt,
                SettingId = user.SettingId
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
                    IsActive = category.IsActive,
                    IsDeleted = category.IsDeleted,
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
                IsActive = category.IsActive,
                IsDeleted = category.IsDeleted,
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
        public static SettingModel ReturnModel(this Setting setting)
        {
            return new SettingModel()
            {
                Id = setting.Id,
                Name = setting.Name,
                Logo = setting.Logo,
                PhoneNumber = setting.PhoneNumber,
                Email = setting.Email,
                Address = setting.Address,
                GoogleClientId = setting.GoogleClientId,
                GoogleClientSecretId = setting.GoogleClientSecretId,
                GoogleFolderId = setting.GoogleFolderId,
                GoogleToken = setting.GoogleToken
            };
        }
    }
}
