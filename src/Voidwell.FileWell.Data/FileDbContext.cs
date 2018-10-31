using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.RegularExpressions;
using Voidwell.FileWell.Data.Models;

namespace Voidwell.FileWell.Data
{
    public class FileDbContext : DbContext
    {
        public FileDbContext(DbContextOptions<FileDbContext> options) : base(options)
        {
        }

        public DbSet<FileRecord> Files { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ConvertToConvention(builder);
        }

        private static void ConvertToConvention(ModelBuilder builder)
        {
            foreach (var entity in builder.Model.GetEntityTypes())
            {
                entity.Relational().TableName = ToSnakeCase(entity.Relational().TableName);

                foreach (var property in entity.GetProperties())
                {
                    property.Relational().ColumnName = ToSnakeCase(property.Name);
                }

                foreach (var key in entity.GetKeys())
                {
                    key.Relational().Name = $"pk_{ToSnakeCase(key.Relational().Name)}";
                }

                foreach (var key in entity.GetForeignKeys())
                {
                    key.Relational().Name = $"fk_{ToSnakeCase(key.Relational().Name)}";
                }

                foreach (var index in entity.GetIndexes())
                {
                    index.Relational().Name = $"ix_{ToSnakeCase(index.Relational().Name)}";
                }
            }
        }

        private static string ToSnakeCase(string input)
        {
            var matches = Regex.Matches(input, "([A-Z][A-Z0-9]*(?=$|[A-Z][a-z0-9])|[A-Za-z][a-z0-9]+)");
            var result = string.Join("_", matches.Select(a => a.Value));

            return result.ToLower();
        }
    }
}
