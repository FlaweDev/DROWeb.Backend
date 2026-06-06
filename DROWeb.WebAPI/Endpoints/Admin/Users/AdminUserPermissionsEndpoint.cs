using DROWeb.Application.Interfaces;
using DROWeb.Domain.Models;
using DROWeb.Application.Services;
using FastEndpoints;

namespace DROWeb.WebAPI.Endpoints.Admin.Users;

public class AdminUserPermissionsEndpoint : Endpoint<AdminUpdatePermissionsRequest, AdminPermissionResponse>
{
    private readonly PermissionService _permissionService;

    public AdminUserPermissionsEndpoint(PermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    public override void Configure()
    {
        Put("/api/admin/users/{Id}/permissions");
        Claims("Admin");
        Description(x => x.WithName("AdminUpdatePermissions"));
    }

    public override async Task HandleAsync(AdminUpdatePermissionsRequest req, CancellationToken ct)
    {
        var success = await _permissionService.SetPermissionsAsync(req.Id, req.Permissions, ct);
        if (!success)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(new AdminPermissionResponse(req.Id, req.Permissions), ct);
    }
}

public record AdminUpdatePermissionsRequest(Guid Id, Permission Permissions);

public record AdminPermissionResponse(Guid Id, Permission Permissions);
