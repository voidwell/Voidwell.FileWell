using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Voidwell.FileWell.Data.Repositories;

namespace Voidwell.FileWell.Data
{
    public static class DatabaseExtensions
    {
        private static string _migrationAssembly = typeof(DatabaseExtensions).GetTypeInfo().Assembly.GetName().Name;

        public static IServiceCollection AddEntityFrameworkContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<DatabaseOptions>(configuration);

            var options = configuration.Get<DatabaseOptions>();

            services.AddEntityFrameworkNpgsql();

            services.AddDbContextPool<FileDbContext>(builder =>
                builder.UseNpgsql(options.ConnectionString, b => {
                    b.MigrationsAssembly(_migrationAssembly);
                }), 5);

            services.AddTransient<IFileRepository, FileRepository>();

            return services;
        }
    }
}
