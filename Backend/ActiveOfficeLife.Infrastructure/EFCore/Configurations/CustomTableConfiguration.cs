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
    public class CustomTableConfiguration : IEntityTypeConfiguration<CustomTable>
    {
        public void Configure(EntityTypeBuilder<CustomTable> builder)
        {
            builder.ToTable("CustomTables");
            builder.HasKey(ct => ct.Id);
            builder.Property(ct => ct.Id)
                   .ValueGeneratedOnAdd();
            builder.Property(ct => ct.Name)
                .IsRequired()
                .HasMaxLength(255);
            builder.Property(ct => ct.Description)
                .HasMaxLength(1000);
            builder.Property(ct => ct.Type)
                .HasMaxLength(100);
            builder.Property(ct => ct.Data)
                .HasColumnType("nvarchar(max)");
            builder.Property(ct => ct.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
            builder.Property(ct => ct.StartDate)
                .HasDefaultValue(DateTime.MinValue);
            builder.Property(ct => ct.EndDate)
                .HasDefaultValue(DateTime.MaxValue);
            builder.Property(ct => ct.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(ct => ct.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
