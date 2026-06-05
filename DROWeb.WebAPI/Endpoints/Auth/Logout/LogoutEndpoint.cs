using FastEndpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace DROWeb.WebAPI.Endpoints.Auth.Logout;

public class Logout : EndpointWithoutRequest
{
    public override void Configure()
    {
        Post("/api/auth/logout");
        Description(x => x.WithName("Logout"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await Send.NoContentAsync(ct);
    }
}
