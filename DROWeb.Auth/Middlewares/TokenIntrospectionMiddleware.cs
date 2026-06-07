using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace DROWeb.Auth.Middlewares
{
    public class TokenIntrospectionMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenIntrospectionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IMemoryCache cache)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userId = context.User.FindFirst("UserId")?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    string blacklistKey = $"revoked_session_{userId}";

                    if (cache.TryGetValue(blacklistKey, out bool isRevoked) && isRevoked)
                    {
                        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                        cache.Remove(blacklistKey);

                        //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.Redirect("/api/auth/login");
                        return;;
                    }
                }
            }

            await _next(context);
        }
    }
}
