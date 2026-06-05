using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace DROWeb.Auth.Interfaces
{
    public interface IAuthProvider
    {
        string Scheme { get; }
        string DisplayName { get; }
    }

    public interface IAuthProvider<TOptions> : IAuthProvider where TOptions : OAuthOptions, new()
    {
        void Setup(TOptions options, IConfiguration config);
    }
}
