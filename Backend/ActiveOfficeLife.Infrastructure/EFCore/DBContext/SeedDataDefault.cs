using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Infrastructure.EFCore.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Infrastructure.EFCore.DBContext
{
    public static class ModelBuilderExtensions
    {
        public static void SeedDataDefault(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Setting>().HasData(new Setting()
            {
                Id = Guid.NewGuid(),
                Name = "Active Office Life"
            });
            Guid roleAdminId = Guid.NewGuid();
            Guid roleUserId = Guid.NewGuid();
            Guid roleCommentId = Guid.NewGuid();
            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    Id = roleAdminId,
                    Name = "Admin",
                    Description = "Quản trị viên hệ thống"
                },
                new Role
                {
                    Id = roleUserId,
                    Name = "User",
                    Description = "Người dùng bình thường"
                },
                new Role
                {
                    Id = roleCommentId,
                    Name = "Comment",
                    Description = "Người đọc chỉ comment"
                }
            );
            // new user admin
            Guid adminUserId = Guid.NewGuid();
            Guid userUserId = Guid.NewGuid();
            Guid customUserId = Guid.NewGuid();
            var hasher = new PasswordHasher<User>();
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = adminUserId,
                    Username = "admin",
                    PasswordHash = hasher.HashPassword(null, "admin123"),
                    Email = "activeofficelife@gmail.com",
                },
                new User
                {
                    Id = userUserId,
                    Username = "user",
                    PasswordHash = hasher.HashPassword(null, "user123"),
                    Email = "activeoffice@gmail.com",
                },
                new User
                {
                    Id = customUserId,
                    Username = "custom",
                    PasswordHash = hasher.HashPassword(null, "custom123"),
                    Email = "nguyenquynhvp.ictu@gmail.com"
                }
            );
            // Seed bảng trung gian UserRoles (vì EF Core không tự seeding navigation property)
            modelBuilder.Entity("UserRoles").HasData(
                new { UserId = adminUserId, RoleId = roleAdminId },
                new { UserId = adminUserId, RoleId = roleUserId },
                new { UserId = userUserId, RoleId = roleUserId },
                new { UserId = customUserId, RoleId = roleUserId }
            );
        }
    }
}
