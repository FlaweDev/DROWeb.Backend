using DROWeb.Application.Interfaces;
using DROWeb.Domain.Models;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace DROWeb.WebAPI.Endpoints.Auth.Status;

public class Status : EndpointWithoutRequest<StatusResponse>
{
    private readonly IUsersDbContext _dbContext;

    public Status(IUsersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Get("/api/auth/status");
        AllowAnonymous();
        Description(x => x.WithName("Status"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var isAuthenticated = User?.Identity?.IsAuthenticated ?? false;

        if (!isAuthenticated)
        {
            await Send.OkAsync(new StatusResponse(false, Guid.Empty, string.Empty, false, string.Empty), ct);
            return;
        }

        var userIdClaim = User.FindFirst("AppUserId");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            await Send.OkAsync(new StatusResponse(false, Guid.Empty, string.Empty, false, string.Empty), ct);
            return;
        }

        var user = await _dbContext.Users
    .Include(u => u.ExternalAuths)
    .FirstOrDefaultAsync(u => u.Id == userId, ct);

        await Send.OkAsync(new StatusResponse(true, user?.Id ?? Guid.Empty, user?.Username ?? string.Empty, user?.IsAdmin ?? false, user?.ExternalAuths?.FirstOrDefault()?.AvatarHash ?? ""), ct);
    }
}

public record NoRequest;
public record StatusResponse(bool IsAuthenticated, Guid UserId, string Username, bool IsAdmin, string AvatarHash);
