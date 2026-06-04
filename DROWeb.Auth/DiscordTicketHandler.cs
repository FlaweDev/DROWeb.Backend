using DROWeb.Domain.Events;
using MediatR;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace DROWeb.Auth;

public class DiscordTicketHandler
{
    public Func<OAuthCreatingTicketContext, Task> OnCreatingTicket { get; set; } = async context =>
    {
        var mediator = context.HttpContext.RequestServices.GetRequiredService<IMediator>();
        var userIdentity = (ClaimsIdentity)context.Principal!.Identity!;

        var discordId = userIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var username = userIdentity.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(discordId) || string.IsNullOrEmpty(username))
            return;

        var userAuthenticated = new UserAuthenticated
        {
            UserId = Guid.NewGuid(),
            Username = username,
            ExternalId = discordId,
            Provider = "Discord"
        };

        await mediator.Publish(userAuthenticated, context.HttpContext.RequestAborted);
    };
}
