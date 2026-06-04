using FastEndpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DROWeb.WebAPI.Endpoints.Auth.Home;

public class HomePageEndpoint : EndpointWithoutRequest
{
    private readonly IWebHostEnvironment _env; 
    
    public HomePageEndpoint(IWebHostEnvironment env)
    {
        _env = env;
    }

    public override void Configure()
    {
        Get("/");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var filePath = Path.Combine(_env.WebRootPath, "index.html");

        var fileInfo = new FileInfo(filePath);

        if (!fileInfo.Exists)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.FileAsync(fileInfo, contentType: "text/html", cancellation: ct);
    }
}

public record EmptyRequest;
