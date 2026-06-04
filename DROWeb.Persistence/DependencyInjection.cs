using DROWeb.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace DROWeb.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services,
            IConfiguration config)
        {
            var connectionString = new NpgsqlConnectionStringBuilder
            {
                Host = config["DATABASE_HOST"],
                Port = int.TryParse(config["DATABASE_PORT"], out var p) ? p : 5432,
                Database = config["DATABASE_NAME"],
                Username = config["DATABASE_USER"],
                Password = config["DATABASE_PASSWORD"],
                Pooling = true
            }.ConnectionString;

            services.AddDbContext<UsersDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            services.AddScoped<IUsersDbContext>(provider =>
                provider.GetRequiredService<UsersDbContext>());

            return services;
        }
    }
}
