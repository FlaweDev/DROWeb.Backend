using DROWeb.Application.Interfaces;
using DROWeb.Domain.Events;
using DROWeb.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DROWeb.Application.Services;

public class UserAuthenticationService : INotificationHandler<UserAuthenticated>
{
    private readonly IUsersDbContext _dbContext;

    public UserAuthenticationService(IUsersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(UserAuthenticated notification, CancellationToken ct)
    {
        var externalAuth = await _dbContext.ExternalAuths
            .FirstOrDefaultAsync(e => e.Provider == notification.Provider && e.ProviderId == notification.ExternalId, ct);

        User user;

        if (externalAuth == null)
        {
            user = new User
            {
                Id = notification.UserId,
                Username = notification.Username,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Users.Add(user);

            var newExternalAuth = new ExternalAuth
            {
                Id = Guid.NewGuid(),
                UserId = notification.UserId,
                Provider = notification.Provider,
                ProviderId = notification.ExternalId ?? string.Empty,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.ExternalAuths.Add(newExternalAuth);
        }
        else
        {
            user = await _dbContext.Users
                .Include(u => u.ExternalAuths)
                .FirstAsync(u => u.Id == externalAuth.UserId, ct);

            if (user.Username != notification.Username)
            {
                user.Username = notification.Username;
            }
        }

        await _dbContext.SaveChangesAsync(ct);
    }
}
