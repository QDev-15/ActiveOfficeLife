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
    public class SeoMetadataConfiguration : IEntityTypeConfiguration<SeoMetadata>
    {
        public void Configure(EntityTypeBuilder<SeoMetadata> builder)
        {
            builder.ToTable("SeoMetaData");
            builder.HasKey(s => s.Id);

            builder.Property(s => s.MetaTitle)
                   .HasMaxLength(300);

            builder.Property(s => s.MetaDescription)
                   .HasMaxLength(1000);

            builder.Property(s => s.MetaKeywords)
                   .HasMaxLength(500);
        }
    }
}
