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
    public class AffiliateConfiguration : IEntityTypeConfiguration<Affiliate>
    {
        public void Configure(EntityTypeBuilder<Affiliate> builder)
        {
            builder.ToTable("Affiliates");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id)
                   .ValueGeneratedOnAdd();
            builder.Property(a => a.Name)
                     .IsRequired()
                     .HasMaxLength(200);
            builder.Property(a => a.Url)
                        .HasMaxLength(500);
            builder.Property(a => a.ShopName)
                        .HasMaxLength(200);
            builder.Property(a => a.ShopId)
                        .HasMaxLength(100);
            builder.Property(a => a.ShopAvatar)
                        .HasMaxLength(500);
            builder.Property(a => a.Description)
                        .HasMaxLength(2000);
            builder.Property(a => a.IsActive)
                     .HasDefaultValue(true);
            builder.Property(a => a.IsDeleted)
                        .HasDefaultValue(false);
            builder.Property(a => a.CreatedAt)
                        .IsRequired()
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(a => a.UpdatedAt)
                        .IsRequired()
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
