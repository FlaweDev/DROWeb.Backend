using DROWeb.Application.Interfaces;
using DROWeb.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using MediatR;

namespace DROWeb.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            //services.AddScoped<IPlayerService, PlayerService>();
            services.AddScoped<UserAuthenticationService>();
            services.AddScoped<IAvatarService, AvatarService>();

            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

            return services;
        }
    }
}
