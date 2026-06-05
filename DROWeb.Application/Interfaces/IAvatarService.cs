using System;
using System.Collections.Generic;
using System.Text;

namespace DROWeb.Application.Interfaces
{
    public interface IAvatarService
    {
        Task<string> GetAvatarUrl(Guid userId, CancellationToken ct = default);
    }
}
