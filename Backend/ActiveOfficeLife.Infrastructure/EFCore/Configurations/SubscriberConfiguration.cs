using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Infrastructure.EFCore.Configurations
{
    public class SubscriberConfiguration : IEntityTypeConfiguration<Domain.Entities.Subscriber>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Domain.Entities.Subscriber> builder)
        {
            builder.ToTable("Subscribers");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id)
                   .ValueGeneratedOnAdd();
            builder.Property(s => s.Email)
                   .IsRequired()
                   .HasMaxLength(200);
            builder.Property(s => s.SubscribedAt)
                   .IsRequired();
        }
    }
}
