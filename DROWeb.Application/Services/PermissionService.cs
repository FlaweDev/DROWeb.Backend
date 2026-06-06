using DROWeb.Application.Interfaces;
using DROWeb.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DROWeb.Application.Services;

public class PermissionService
{
    private readonly IUsersDbContext _dbContext;

    public PermissionService(IUsersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetUserByIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await _dbContext.Users
            .Include(u => u.ExternalAuths)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);
    }

    public async Task<bool> AddPermissionAsync(Guid userId, Permission permission, CancellationToken ct = default)
    {
        var user = await _dbContext.Users.FindAsync(new object[] { userId }, ct);
        if (user == null)
            return false;

        user.Permissions |= permission;
        await _dbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> RemovePermissionAsync(Guid userId, Permission permission, CancellationToken ct = default)
    {
        var user = await _dbContext.Users.FindAsync(new object[] { userId }, ct);
        if (user == null)
            return false;

        user.Permissions &= ~permission;
        await _dbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> SetPermissionsAsync(Guid userId, Permission permissions, CancellationToken ct = default)
    {
        var user = await _dbContext.Users.FindAsync(new object[] { userId }, ct);
        if (user == null)
            return false;

        user.Permissions = permissions;
        await _dbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> HasPermissionAsync(Guid userId, Permission permission, CancellationToken ct = default)
    {
        var user = await _dbContext.Users
            .Where(u => u.Id == userId)
            .Select(u => u.Permissions)
            .FirstOrDefaultAsync(ct);

        return user != null && (user & permission) != 0;
    }

    public async Task<bool> HasAnyPermissionAsync(Guid userId, Permission permissions, CancellationToken ct = default)
    {
        var user = await _dbContext.Users
            .Where(u => u.Id == userId)
            .Select(u => u.Permissions)
            .FirstOrDefaultAsync(ct);

        return user != null && (user & permissions) != Permission.None;
    }

    public async Task<bool> HasAllPermissionsAsync(Guid userId, Permission permissions, CancellationToken ct = default)
    {
        var user = await _dbContext.Users
            .Where(u => u.Id == userId)
            .Select(u => u.Permissions)
            .FirstOrDefaultAsync(ct);

        return user != null && (user & permissions) == permissions;
    }
}
