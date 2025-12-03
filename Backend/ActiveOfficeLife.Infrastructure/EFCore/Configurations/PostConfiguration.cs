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
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable("Posts");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id)
                   .ValueGeneratedOnAdd();

            builder.Property(p => p.Title)
                   .IsRequired()
                   .HasMaxLength(300);

            builder.Property(p => p.Slug)
                   .IsRequired()
                   .HasMaxLength(300);

            builder.Property(p => p.Summary)
                   .HasMaxLength(1000);

            builder.Property(p => p.Status)
                   .HasConversion<string>() // Lưu Enum dưới dạng string
                   .HasMaxLength(50);

            builder.HasOne(p => p.Author)
                   .WithMany(u => u.Posts)
                   .HasForeignKey(p => p.AuthorId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Category)
                   .WithMany(c => c.Posts)
                   .HasForeignKey(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Tags)
     .WithMany(t => t.Posts)
     .UsingEntity<Dictionary<string, object>>(
         "PostTags",  // Tên bảng trung gian
         j => j
             .HasOne<Tag>()
             .WithMany()
             .HasForeignKey("TagId")
             .OnDelete(DeleteBehavior.Cascade),
         j => j
             .HasOne<Post>()
             .WithMany()
             .HasForeignKey("PostId")
             .OnDelete(DeleteBehavior.Cascade),
         j =>
         {
             j.HasKey("PostId", "TagId");
             j.ToTable("PostTags");
 
             j.HasIndex("TagId");
             j.HasIndex("PostId");
         }
     );


            // Quan hệ 1–1 với SeoMetadata
            builder.HasOne(p => p.SeoMetadata)
                   .WithOne()
                   .HasForeignKey<Post>(p => p.SeoMetadataId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }

}
