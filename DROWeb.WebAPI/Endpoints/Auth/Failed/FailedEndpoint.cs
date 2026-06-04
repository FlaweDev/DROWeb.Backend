using FastEndpoints;

namespace DROWeb.WebAPI.Endpoints.Auth.Failed;

public class Failed : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/api/auth/failed");
        AllowAnonymous();
        Description(x => x.WithName("AuthFailed"));
    }

    public override Task HandleAsync(CancellationToken ct)
    {
        return Send.NoContentAsync();
    }
}

public record NoRequest;
public record EmptyResponse;
