using DROWeb.Application.Interfaces;
using DROWeb.Domain.Models;
using DROWeb.WebAPI.Endpoints.Auth.Status;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DROWeb.WebAPI.Endpoints.Auth.Callback;

public class Callback : Endpoint<NoRequest>
{
    private readonly IUsersDbContext _dbContext;

    public Callback(IUsersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Get("/api/auth/callback");
        AllowAnonymous();
        Description(x => x.WithName("Callback"));
    }

    public override async Task HandleAsync(NoRequest req, CancellationToken ct)
    {
        // Authenticate to read the cookie
        var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!authenticateResult.Succeeded)
        {
            HttpContext.Response.Redirect("/api/auth/failed");
            return;
        }

        // Get user info from claims
        var userIdClaim = authenticateResult.Principal?.FindFirst("AppUserId");
        if (userIdClaim == null)
        {
            HttpContext.Response.Redirect("/api/auth/failed");
            return;
        }

        var userId = Guid.Parse(userIdClaim.Value);
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user == null)
        {
            HttpContext.Response.Redirect("/api/auth/failed");
            return;
        }

        // Re-sign in with app user data
        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        identity.AddClaim(new Claim(ClaimTypes.Name, user.Username));
        identity.AddClaim(new Claim("AppUserId", user.Id.ToString()));

        var props = new AuthenticationProperties
        {
            IsPersistent = true,
            AllowRefresh = true
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity), props);

        // Redirect back to the return URL or home
        var returnUrl = HttpContext.Request.Query.TryGetValue("returnUrl", out var url) ? url.ToString() : "/";

        HttpContext.Response.Redirect(returnUrl);
    }
}

public record NoRequest;
