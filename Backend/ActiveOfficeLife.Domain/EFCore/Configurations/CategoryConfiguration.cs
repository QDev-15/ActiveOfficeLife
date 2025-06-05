using ActiveOfficeLife.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Domain.EFCore.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(c => c.Slug)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(c => c.Description)
                   .HasMaxLength(1000);

            // Quan hệ cha-con
            builder.HasOne(c => c.Parent)
                   .WithMany(c => c.Children)
                   .HasForeignKey(c => c.ParentId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ 1-1 với SeoMetadata
            builder.HasOne(c => c.SeoMetadata)
                   .WithOne()
                   .HasForeignKey<Category>(c => c.SeoMetadataId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
