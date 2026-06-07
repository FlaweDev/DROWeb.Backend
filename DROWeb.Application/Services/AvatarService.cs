using DROWeb.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DROWeb.Application.Services
{
    public class AvatarService : IAvatarService
    {
        private readonly IEnumerable<IAvatarUrlStrategy> _strategies;
        private readonly IUsersDbContext _context;

        public AvatarService(IEnumerable<IAvatarUrlStrategy> strategies, IUsersDbContext context)
        {
            _strategies = strategies;
            _context = context;
        }

        public async Task<string> GetAvatarUrl(Guid userId, CancellationToken ct = default)
        {
            var user = await _context.Users.Include(x => x.ExternalAuths).FirstAsync(x => x.Id == userId, ct);

            var externalAuth = user.ExternalAuths.FirstOrDefault();
            if (externalAuth != null)
            {
                var strategy = _strategies.FirstOrDefault(s => s.ProviderName == externalAuth.Provider);

                if (strategy != null)
                    return strategy.BuildAvatarUrl(externalAuth.ProviderId, externalAuth.AvatarHash);
                else
                    throw new NotSupportedException($"Provider {externalAuth.Provider} is not supported.");
            }
            else
            {
                throw new NotSupportedException($"User not using externalAuth.");
            }
        }
    }
}
