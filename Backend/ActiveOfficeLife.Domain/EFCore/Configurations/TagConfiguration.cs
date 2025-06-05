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
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.ToTable("Tags");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Slug)
                .IsRequired()
                .HasMaxLength(200);

            // Quan hệ 1-1 với SeoMetadata
            builder.HasOne(t => t.SeoMetadata)
                .WithOne()
                .HasForeignKey<Tag>(t => t.SeoMetadataId)
                .OnDelete(DeleteBehavior.SetNull); // hoặc Cascade nếu muốn xóa SeoMetadata khi xóa Tag

            // Many-to-Many với Post
            builder.HasMany(t => t.Posts)
                .WithMany(p => p.Tags);
        }
    }
}
