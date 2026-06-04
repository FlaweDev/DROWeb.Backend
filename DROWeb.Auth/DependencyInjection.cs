using AspNet.Security.OAuth.Discord;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DROWeb.Auth;

public static class DependencyInjection
{
    public static IServiceCollection AddAuth(this IServiceCollection services,
        IConfiguration config)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = DiscordAuthenticationDefaults.AuthenticationScheme;
        })
            .AddCookie(options =>
            {
                options.LoginPath = "/api/auth/login";
                options.LogoutPath = "/api/auth/logout";
            })
            .AddDiscord(options =>
            {
                options.ClientId = config["CLIENT_ID"] ?? throw new InvalidOperationException("Discord ClientId is missing.");
                options.ClientSecret = config["CLIENT_SECRET"] ?? throw new InvalidOperationException("Discord ClientSecret is missing.");

                options.Scope.Add("identify");

                options.SaveTokens = true;

                options.Events.OnCreatingTicket = new DiscordTicketHandler().OnCreatingTicket;
            });

        return services;
    }
}
