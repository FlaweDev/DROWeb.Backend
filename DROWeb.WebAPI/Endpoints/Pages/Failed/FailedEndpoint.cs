using FastEndpoints;

namespace DROWeb.WebAPI.Endpoints.Pages.Failed;

public class FailedPageEndpoint : EndpointWithoutRequest
{
   
    private readonly IWebHostEnvironment _env;

    public FailedPageEndpoint(IWebHostEnvironment env)
    {
        _env = env;
    }

    public override void Configure()
    {
        Get("/failed");
        AllowAnonymous();
        Description(x => x.WithName("AuthFailed"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var filePath = Path.Combine(_env.WebRootPath, "failed.html");

        if (!File.Exists(filePath))
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        HttpContext.Response.ContentType = "text/html; charset=utf-8";
        await HttpContext.Response.SendFileAsync(filePath, ct);
    }
}

