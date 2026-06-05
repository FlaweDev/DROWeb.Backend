using DROWeb.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DROWeb.Auth.Avatars
{
    public class DiscordAvatarUrlStrategy : IAvatarUrlStrategy
    {
        public string ProviderName => "Discord";

        public string BuildAvatarUrl(string providerUserId, string avatarHash, int size = 256)
        {
            if (string.IsNullOrEmpty(avatarHash))
                return "https://cdn.discordapp.com/embed/avatars/0.png";

            return $"https://cdn.discordapp.com/avatars/{providerUserId}/{avatarHash}?size={size}";
        }
    }
}
