using DROWeb.Application.Interfaces;
using DROWeb.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DROWeb.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IPlayerService, PlayerService>();

            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}
