using AspNet.Security.OAuth.Discord;
using DROWeb.Application.Interfaces;
using DROWeb.Application.Services;
using DROWeb.Auth.Avatars;
using DROWeb.Auth.Middlewares;
using DROWeb.Auth.Providers;
using DROWeb.Domain.Events;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace DROWeb.Auth;

public static class DependencyInjection
{
    public static IServiceCollection AddAuth(this IServiceCollection services,
        IConfiguration config)
    {
        if (!double.TryParse(config["IdentitySettings:CookieExpireTimeSpanInHours"], out var cookieExpireTime))
            throw new InvalidOperationException(
                "Критическая ошибка конфигурации: Значение 'IdentitySettings:CookieExpireTimeSpanInHours' не найдено или не является валидным числом типа double."
            );

        services.AddAuthentication(options =>
        {
            //options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //options.DefaultChallengeScheme = DiscordAuthenticationDefaults.AuthenticationScheme;

            options.DefaultChallengeScheme = "Discord";
        })
            .AddCookie(options =>
            {
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.HttpOnly = true;

                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(config.GetValue<int>("IdentitySettings:CookieExpireTimeSpanInHours"));
            })
            .AddDiscord("Discord", options =>
            {
                var discord = new DiscordProvider();
                discord.Setup(options, config);
            });


        services.AddScoped<IAvatarUrlStrategy, DiscordAvatarUrlStrategy>();

        return services;
    }

    public static IApplicationBuilder UseTokenIntrospection(this IApplicationBuilder app)
    {
        return app.UseMiddleware<TokenIntrospectionMiddleware>();
    }
}
