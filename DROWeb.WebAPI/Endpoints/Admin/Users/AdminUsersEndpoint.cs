using DROWeb.Application.Interfaces;
using DROWeb.Domain.Models;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace DROWeb.WebAPI.Endpoints.Admin.Users;

public class AdminUsersEndpoint : EndpointWithoutRequest<AdminUserResponse[]>
{
    private readonly IUsersDbContext _dbContext;

    public AdminUsersEndpoint(IUsersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Get("/api/admin/users");
        Claims("Admin");
        Description(x => x.WithName("AdminListUsers"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var search = HttpContext.Request.Query.TryGetValue("search", out var searchValue) ? searchValue.ToString() : "";
        var provider = HttpContext.Request.Query.TryGetValue("provider", out var providerValue) ? providerValue.ToString() : "";

        var query = _dbContext.Users
            .Include(u => u.ExternalAuths)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(search))
        {
            if (string.IsNullOrWhiteSpace(provider))
            {
                // Search by user ID or ProviderId across all providers
                if (Guid.TryParse(search, out var userId))
                {
                    query = query.Where(u => u.Id == userId);
                }
                else
                {
                    // Search by ProviderId
                    query = query.Where(u => u.ExternalAuths.Any(ea => ea.ProviderId.Contains(search)));
                }
            }
            else
            {
                // Search by ProviderId for specific provider
                query = query.Where(u => u.ExternalAuths.Any(ea => ea.Provider == provider && ea.ProviderId.Contains(search)));
            }
        }

        var users = await query.ToListAsync(ct);

        var response = users.Select(user =>
        {
            // If provider filter is set, get that specific auth
            // Otherwise get the first auth
            var auth = string.IsNullOrWhiteSpace(provider)
                ? user.ExternalAuths.FirstOrDefault()
                : user.ExternalAuths.FirstOrDefault(ea => ea.Provider == provider);

            return new AdminUserResponse(
                user.Id,
                user.Username,
                auth?.Provider ?? "",
                auth?.ProviderId ?? "",
                user.Permissions
            );
        }).ToArray();

        await Send.OkAsync(response, ct);
    }
}

public record AdminUserResponse(Guid Id, string Username, string Provider, string ProviderId, Permission Permissions);
