using AspNet.Security.OAuth.Discord;
using DROWeb.Application.Interfaces;
using DROWeb.Auth.Avatars;
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
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace DROWeb.Auth;

public static class DependencyInjection
{
    public static IServiceCollection AddAuth(this IServiceCollection services,
        IConfiguration config)
    {
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
                //options.LoginPath = "/api/auth/login";
                //options.LogoutPath = "/api/auth/logout";
            })
            .AddDiscord("Discord", options =>
            {
                var discord = new DiscordProvider();
                discord.Setup(options, config);
            });





        services.AddScoped<IAvatarUrlStrategy, DiscordAvatarUrlStrategy>();

        return services;
    }
}
