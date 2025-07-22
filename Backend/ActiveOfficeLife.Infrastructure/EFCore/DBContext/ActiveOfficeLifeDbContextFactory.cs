using ActiveOfficeLife.Domain.EFCore.DBContext;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Infrastructure.EFCore.DBContext
{
    public class ActiveOfficeLifeDbContextFactory : IDesignTimeDbContextFactory<ActiveOfficeLifeDbContext>
    {
        public ActiveOfficeLifeDbContext CreateDbContext(string[] args)
        {
            // Build configuration
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Lấy folder hiện tại khi gọi từ CLI
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<ActiveOfficeLifeDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new ActiveOfficeLifeDbContext(optionsBuilder.Options);
        }
    }
}
