using ActiveOfficeLife.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Domain.EFCore.Configurations
{
    public class MediaConfiguration : IEntityTypeConfiguration<Media>
    {
        public void Configure(EntityTypeBuilder<Media> builder)
        {
            builder.ToTable("Medias");

            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id)
                   .ValueGeneratedOnAdd();

            builder.Property(m => m.FileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(m => m.FilePath)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(m => m.FileType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.FileSize)
                .IsRequired();

            builder.Property(m => m.MediaType)
                .HasConversion<string>() // Lưu enum dưới dạng string
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(m => m.UploadedAt)
                .IsRequired();

            builder.HasOne(m => m.UploadedBy)
                .WithMany()
                .HasForeignKey(m => m.UploadedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
