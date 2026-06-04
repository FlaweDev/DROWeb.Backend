using FastEndpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DROWeb.WebAPI.Endpoints.Auth.Login;

public class Login : Endpoint<EmptyRequest, NoContent>
{
    public override void Configure()
    {
        Post("/api/auth/login");
        AllowAnonymous();
        Description(x => x.WithName("Login"));
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var redirectUrl = HttpContext.Request.Query.TryGetValue("returnUrl", out var url) ? url.ToString() : "/";
        await HttpContext.ChallengeAsync("Discord", new AuthenticationProperties { RedirectUri = redirectUrl });
    }
}

public record EmptyRequest;
