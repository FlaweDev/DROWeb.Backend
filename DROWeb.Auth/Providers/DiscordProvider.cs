using AspNet.Security.OAuth.Discord;
using DROWeb.Application.Interfaces;
using DROWeb.Application.Services;
using DROWeb.Auth.Interfaces;
using DROWeb.Domain.Events;
using DROWeb.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace DROWeb.Auth.Providers
{
    public class DiscordProvider : OAuthAuthProvider<DiscordAuthenticationOptions>
    {
        
        protected override void ConfigureProvider(DiscordAuthenticationOptions options, IConfiguration config)
        {
            options.ClientId = config["CLIENT_ID"] ?? throw new InvalidOperationException("Discord ClientId is missing.");
            options.ClientSecret = config["CLIENT_SECRET"] ?? throw new InvalidOperationException("Discord ClientSecret is missing.");

            options.Scope.Add("identify");
        }

        protected override async Task OnCreatingTicket(OAuthCreatingTicketContext context)
        {
            var discordId = context.Identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = context.Identity?.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(discordId) || string.IsNullOrEmpty(username))
            {
                return;
            }

            var mediator = context.HttpContext.RequestServices.GetRequiredService<IMediator>();
            var userAuthenticated = new UserAuthenticated
            {
                UserId = Guid.NewGuid(),
                Username = username,
                ExternalId = discordId,
                Provider = "Discord"
            };

            await mediator.Publish(userAuthenticated, context.HttpContext.RequestAborted);

            var dbContext = context.HttpContext.RequestServices.GetRequiredService<IUsersDbContext>();
            var externalAuth = await dbContext.ExternalAuths
                .FirstOrDefaultAsync(e => e.Provider == "Discord" && e.ProviderId == discordId, context.HttpContext.RequestAborted);

            // Получаем avatar hash из Discord API
            var avatarHash = await GetAvatarHash(context.AccessToken);

            if (externalAuth != null)
            {
                context.Identity.AddClaim(new Claim("AppUserId", externalAuth.UserId.ToString()));

                if (avatarHash != null && externalAuth.AvatarHash != avatarHash)
                {
                    externalAuth.AvatarHash = avatarHash;
                    await dbContext.SaveChangesAsync(context.HttpContext.RequestAborted);
                }

                var permissionService = context.HttpContext.RequestServices.GetRequiredService<PermissionService>();
                if (await permissionService.HasPermissionAsync(externalAuth.UserId, Permission.GameAccess, context.HttpContext.RequestAborted))
                {
                    context.Identity.AddClaim(new Claim("GameAccess", "true"));
                }
                if (await permissionService.HasPermissionAsync(externalAuth.UserId, Permission.ManagePermissions, context.HttpContext.RequestAborted))
                {
                    context.Identity.AddClaim(new Claim("Admin", "true"));
                }
            }
        }

        private async Task<string?> GetAvatarHash(string accessToken)
        {
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                client.DefaultRequestHeaders.UserAgent.TryParseAdd("Mozilla/5.0");

                var response = await client.GetAsync("https://discord.com/api/users/@me");
                if (response.IsSuccessStatusCode)
                {
                    var jsonDoc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
                    return jsonDoc.RootElement.GetProperty("avatar").GetString();
                }
            }
            catch
            {
                // Игнорируем ошибки получения аватара
            }
            return null;
        }

        protected override Task OnTicketReceived(TicketReceivedContext context)
        {
            return Task.CompletedTask;
        }

        protected override Task OnRemoteFailure(RemoteFailureContext context)
        {
            var queryString = context.Request.QueryString.Value;

            if (queryString == null || queryString.Contains("error=user_cancelled", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.Redirect("/");
                context.HandleResponse();
            }
            else
            {
                var errorMessage = context.Failure?.Message;
                var innerException = context.Failure?.InnerException;

                Console.WriteLine($"[AUTH ERROR] Произошла ошибка: {errorMessage}");
                context.Response.Redirect("/failed");
                context.HandleResponse();
            }

            return Task.CompletedTask;
        }
    }
}
