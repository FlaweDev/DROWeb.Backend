using DROWeb.Application.Services;
using DROWeb.Domain.Models;
using FastEndpoints;

namespace DROWeb.WebAPI.Endpoints.Users.Permissions;

public class PermissionsEndpoint : Endpoint<PermissionRequest>
{
    private readonly PermissionService _permissionService;

    public PermissionsEndpoint(PermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    public override void Configure()
    {
        Put("/api/users/permissions");
        Description(x => x.WithName("UpdatePermissions"));
    }

    public override async Task HandleAsync(PermissionRequest req, CancellationToken ct)
    {
        if (!req.Permissions.HasValue)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var success = await _permissionService.SetPermissionsAsync(req.UserId, req.Permissions.Value, ct);
        if (!success)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(new PermissionResponse(req.UserId, req.Permissions.Value), ct);
    }
}

public record PermissionRequest(Guid UserId, Permission? Permissions);

public record PermissionResponse(Guid UserId, Permission Permissions);
