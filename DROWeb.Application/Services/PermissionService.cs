using DROWeb.Application.Interfaces;
using DROWeb.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace DROWeb.Application.Services;

public class PermissionService
{
    private readonly IUsersDbContext _dbContext;
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _configuration;

    public PermissionService(IUsersDbContext dbContext, IMemoryCache cache, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _cache = cache;
        _configuration = configuration;
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

        MarkerUserSession(userId);

        return true;
    }

    public async Task<bool> RemovePermissionAsync(Guid userId, Permission permission, CancellationToken ct = default)
    {
        var user = await _dbContext.Users.FindAsync(new object[] { userId }, ct);
        if (user == null)
            return false;

        user.Permissions &= ~permission;
        await _dbContext.SaveChangesAsync(ct);

        MarkerUserSession(userId);

        return true;
    }

    public async Task<bool> SetPermissionsAsync(Guid userId, Permission permissions, CancellationToken ct = default)
    {
        var user = await _dbContext.Users.FindAsync(new object[] { userId }, ct);
        if (user == null)
            return false;

        user.Permissions = permissions;
        await _dbContext.SaveChangesAsync(ct);

        MarkerUserSession(userId);

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

    private void MarkerUserSession(Guid userId)
    {
        string blacklistKey = $"revoked_session_{userId}";

        if (!double.TryParse(_configuration["IdentitySettings:CookieExpireTimeSpanInHours"], out var cookieExpireTime))
            throw new InvalidOperationException(
                "Критическая ошибка конфигурации: Значение 'IdentitySettings:CookieExpireTimeSpanInHours' не найдено или не является валидным числом типа double."
            );

        _cache.Set(blacklistKey, true, TimeSpan.FromHours(cookieExpireTime));
    }
}
