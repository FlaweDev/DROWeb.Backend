using System;
using System.Collections.Generic;
using System.Text;

namespace DROWeb.Application.Interfaces
{
    public interface IAvatarUrlStrategy
    {
        string ProviderName { get; }
        string BuildAvatarUrl(string providerUserId, string avatarHash, int size = 256);
    }
}
