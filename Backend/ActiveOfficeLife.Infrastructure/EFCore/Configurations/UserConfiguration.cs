using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Domain.EFCore.Configurations
{
    using ActiveOfficeLife.Common.Enums;
    using ActiveOfficeLife.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(u => u.PasswordHash)
                .IsRequired();
            builder.Property(u => u.Status)
                .HasConversion<string>() // Chuyển đổi enum sang string
                .HasMaxLength(50) // Giới hạn độ dài của chuỗi
                .HasDefaultValue(UserStatus.Active);

            builder.Property(u => u.Token)
                .HasMaxLength(500);

            builder.Property(u => u.AvatarUrl)
                .HasMaxLength(500);

            builder.Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Quan hệ nhiều-nhiều User - Role (EF Core tự tạo bảng trung gian)
            builder.HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRoles",  // tên bảng trung gian
                    j => j.HasOne<Role>()
                          .WithMany()
                          .HasForeignKey("RoleId")
                          .OnDelete(DeleteBehavior.Cascade),  // xóa UserRoles khi Role bị xóa
                    j => j.HasOne<User>()
                          .WithMany()
                          .HasForeignKey("UserId")
                          .OnDelete(DeleteBehavior.Cascade),  // xóa UserRoles khi User bị xóa
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");  // Khóa chính composite
                        j.ToTable("UserRoles");
                    });

            // Quan hệ 1-nhiều User - Posts
            builder.HasMany(u => u.Posts)
                .WithOne(p => p.Author)
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ 1-nhiều User - Comments
            builder.HasMany(u => u.Comments)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
