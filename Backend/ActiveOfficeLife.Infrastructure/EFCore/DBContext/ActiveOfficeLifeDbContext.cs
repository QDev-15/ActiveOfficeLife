using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Infrastructure.EFCore.Configurations;
using ActiveOfficeLife.Infrastructure.EFCore.DBContext;
using Azure;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Domain.EFCore.DBContext
{
    public class ActiveOfficeLifeDbContext : DbContext
    {
        public ActiveOfficeLifeDbContext(DbContextOptions<ActiveOfficeLifeDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<UserToken> UserTokens => Set<UserToken>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<CategoryType> CategoryTypes => Set<CategoryType>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Media> Media => Set<Media>();
        public DbSet<SeoMetadata> SeoMetadata => Set<SeoMetadata>();
        public DbSet<Visitor> Visitors => Set<Visitor>();
        public DbSet<Ad> Ads => Set<Ad>();
        public DbSet<Log> Logs => Set<Log>();
        public DbSet<Setting> Settings => Set<Setting>();
        public DbSet<CustomTable> CustomTables => Set<CustomTable>();
        public DbSet<Affiliate> Affiliates => Set<Affiliate>();
        public DbSet<Subscriber> Subscribers => Set<Subscriber>();
        public DbSet<Message> Messages => Set<Message>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply all configurations automatically
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ActiveOfficeLifeDbContext).Assembly);
            modelBuilder.SeedDataDefault();
            base.OnModelCreating(modelBuilder);
        }
    }
}
