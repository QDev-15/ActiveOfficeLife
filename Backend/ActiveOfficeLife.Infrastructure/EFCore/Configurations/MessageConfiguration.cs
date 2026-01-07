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
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Messages");
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id)
                   .ValueGeneratedOnAdd();
            builder.Property(m => m.Name).IsRequired().HasMaxLength(200);
            builder.Property(m => m.Email).IsRequired().HasMaxLength(200);
            builder.Property(m => m.Subject).IsRequired().HasMaxLength(300);
            builder.Property(m => m.Content).IsRequired();
        }
    }
}
