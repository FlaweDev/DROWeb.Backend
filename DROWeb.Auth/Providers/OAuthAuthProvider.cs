using DROWeb.Auth.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace DROWeb.Auth.Providers
{
    /// <summary>
    /// Base class for OAuth auth providers. Simplifies setup and event handling.
    /// </summary>
    /// <typeparam name="TOptions">The OAuth options type for this provider</typeparam>
    public abstract class OAuthAuthProvider<TOptions> : IAuthProvider<TOptions> where TOptions : OAuthOptions, new()
    {
        public virtual string Scheme => GetType().Name.Replace("Provider", string.Empty);
        public virtual string DisplayName => Scheme;

        public virtual void Setup(TOptions options, IConfiguration config)
        {
            // Common setup for all providers
            options.CallbackPath = "/api/auth/callback";
            options.SaveTokens = false;
            options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            // Wire up events - providers override protected methods
            options.Events.OnCreatingTicket = OnCreatingTicket;
            options.Events.OnTicketReceived = OnTicketReceived;
            options.Events.OnAccessDenied = OnAccessDenied;
            options.Events.OnRemoteFailure = OnRemoteFailure;

            ConfigureProvider(options, config);
        }


        protected abstract void ConfigureProvider(TOptions options, IConfiguration config);

        protected virtual async Task OnCreatingTicket(OAuthCreatingTicketContext context)
        {
            await HandleCreatingTicket(context);
            await AddCustomClaims(context);
        }
        protected virtual Task OnTicketReceived(TicketReceivedContext context)
        {
            return Task.CompletedTask;
        }

        protected virtual Task HandleCreatingTicket(OAuthCreatingTicketContext context)
        {
            return Task.CompletedTask;
        }

        protected virtual Task AddCustomClaims(OAuthCreatingTicketContext context)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnAccessDenied(AccessDeniedContext context)
        {
            context.Response.Redirect("/");
            context.HandleResponse();
            return Task.CompletedTask;
        }

        protected virtual Task OnRemoteFailure(RemoteFailureContext context)
        {
            context.Response.Redirect("/failed");
            context.HandleResponse(); 
            return Task.CompletedTask;
        }
    }
}
