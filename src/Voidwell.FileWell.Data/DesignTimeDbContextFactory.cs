using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Voidwell.FileWell.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<FileDbContext>
    {
        public FileDbContext CreateDbContext(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("devsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<FileDbContext>();

            var connectionString = configuration.GetValue<string>("ConnectionString");

            builder.UseNpgsql(connectionString, o =>
            {
                o.CommandTimeout(180);
            });

            return new FileDbContext(builder.Options);
        }
    }
}
