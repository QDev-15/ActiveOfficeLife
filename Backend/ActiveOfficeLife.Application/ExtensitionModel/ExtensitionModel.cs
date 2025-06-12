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
                Children = category.Children.Select(c => c.ReturnModel()).ToList(),
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
