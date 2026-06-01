using DROWeb.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DROWeb.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, 
            IConfiguration configuration)
        {
            var connectionString = configuration["DbConnection"];

            services.AddDbContext<PlayersDbContext>(options =>
            {
                options.UseSqlite(connectionString);
            });

            services.AddScoped<IPlayersDbContext>(provider =>
                provider.GetService<PlayersDbContext>());

            return services;
        }
    }
}
