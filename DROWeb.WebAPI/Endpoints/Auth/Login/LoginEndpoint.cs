using FastEndpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DROWeb.WebAPI.Endpoints.Auth.Login;

public class Login : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/api/auth/login");
        AllowAnonymous();
        Description(x => x.WithName("Login"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        // Используем HTTPS для redirect_uri по умолчанию
        var redirectUri = "/";
        var authProps = new AuthenticationProperties { RedirectUri = redirectUri };
        await Send.ResultAsync(Results.Challenge(authProps, new[] { "Discord" }));
    }
}

public record EmptyRequest;
