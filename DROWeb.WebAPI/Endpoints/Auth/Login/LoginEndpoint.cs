using FastEndpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DROWeb.WebAPI.Endpoints.Auth.Login;

public class Login : EndpointWithoutRequest
{
    public override void Configure()
    {
        Post("/api/auth/login");
        AllowAnonymous();
        Description(x => x.WithName("Login"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var redirectUrl = HttpContext.Request.Query.TryGetValue("returnUrl", out var url) ? url.ToString() : "/";
        await HttpContext.ChallengeAsync("Discord", new AuthenticationProperties { RedirectUri = redirectUrl });
    }
}

public record EmptyRequest;
