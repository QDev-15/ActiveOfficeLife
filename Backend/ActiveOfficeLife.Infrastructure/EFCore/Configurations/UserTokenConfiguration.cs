using ActiveOfficeLife.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Infrastructure.EFCore.Configurations
{
    public class UserTokenConfiguration : IEntityTypeConfiguration<UserToken>
    {
        public void Configure(EntityTypeBuilder<UserToken> builder)
        {
            builder.ToTable("UserToken");

            builder.HasKey(t => t.Id);

            builder.HasOne(ut => ut.User)
            .WithMany()
            .HasForeignKey(ut => ut.UserId);
        }
    }
}
