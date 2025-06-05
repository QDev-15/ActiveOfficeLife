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
    public class VisitorConfiguration : IEntityTypeConfiguration<Visitor>
    {
        public void Configure(EntityTypeBuilder<Visitor> builder)
        {
            builder.ToTable("Visitors");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.IpAddress)
                .IsRequired()
                .HasMaxLength(45); // IPv6 max length

            builder.Property(v => v.UserAgent)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(v => v.VisitDate)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
