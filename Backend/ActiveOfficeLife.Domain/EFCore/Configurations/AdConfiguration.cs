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
    public class AdConfiguration : IEntityTypeConfiguration<Ad>
    {
        public void Configure(EntityTypeBuilder<Ad> builder)
        {
            builder.ToTable("Ads");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(a => a.Type)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.ImageUrl)
                .HasMaxLength(500);

            builder.Property(a => a.Link)
                .HasMaxLength(500);

            builder.Property(a => a.StartDate)
                .IsRequired();

            builder.Property(a => a.EndDate)
                .IsRequired();

            builder.Property(a => a.Status)
                .IsRequired()
                .HasDefaultValue(true);
        }
    }
}
