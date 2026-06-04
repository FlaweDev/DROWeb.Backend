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
                options.ReturnUrlParameter = "returnUrl";
            })
            .AddDiscord(options =>
            {
                options.ClientId = config["CLIENT_ID"] ?? throw new InvalidOperationException("Discord ClientId is missing.");
                options.ClientSecret = config["CLIENT_SECRET"] ?? throw new InvalidOperationException("Discord ClientSecret is missing.");

                options.Scope.Add("identify");
                options.CallbackPath = "/api/auth/callback";

                options.SaveTokens = true;

                options.Events.OnCreatingTicket = new DiscordTicketHandler().OnCreatingTicket;
                options.Events.OnAccessDenied = context =>
                {
                    context.Response.Redirect(context.Request.Scheme + "://" + context.Request.Host + "/api/auth/failed");
                    return Task.CompletedTask;
                };
                options.Events.OnRemoteFailure = context =>
                {
                    if (context.Request.Form["error"].FirstOrDefault() == "user_cancelled")
                    {
                        context.Response.Redirect(context.Request.Scheme + "://" + context.Request.Host + "/api/auth/cancelled");
                        context.HandleResponse();
                    }
                    return Task.CompletedTask;
                };
            });

        return services;
    }
}
