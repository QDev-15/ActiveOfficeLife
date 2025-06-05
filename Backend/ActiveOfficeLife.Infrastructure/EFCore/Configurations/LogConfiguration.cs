using ActiveOfficeLife.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Infrastructure.EFCore.Configurations
{
    public class LogConfiguration : IEntityTypeConfiguration<Log>
    {
        public void Configure(EntityTypeBuilder<Log> builder)
        {
            builder.ToTable("Logs");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.Level)
                   .HasConversion<string>() // Chuyển đổi LogLevel sang string
                   .HasMaxLength(100);

            builder.Property(l => l.Message)
                   .IsRequired();

            builder.Property(l => l.StackTrace)
                   .HasColumnType("nvarchar(max)");

            builder.Property(l => l.Source);

            builder.Property(l => l.UserId)
                   .HasMaxLength(100);

            builder.Property(l => l.IpAddress)
                   .HasMaxLength(100);

            builder.Property(l => l.RequestPath)
                   .HasMaxLength(1000);
        }
    }

}
