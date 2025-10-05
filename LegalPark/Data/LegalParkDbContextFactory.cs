using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LegalPark.Data
{
    public class LegalParkDbContextFactory : IDesignTimeDbContextFactory<LegalParkDbContext>
    {
        public LegalParkDbContext CreateDbContext(string[] args)
        {
            
            var basePath = Directory.GetCurrentDirectory();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException($"Connection string 'DefaultConnection' not found in appsettings.json.");
            }

            var builder = new DbContextOptionsBuilder<LegalParkDbContext>();
            builder.UseSqlServer(connectionString);

            return new LegalParkDbContext(builder.Options);
        }
    }
}
