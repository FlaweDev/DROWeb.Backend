using FastEndpoints;

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

        if (!File.Exists(filePath))
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        HttpContext.Response.ContentType = "text/html; charset=utf-8";
        await HttpContext.Response.SendFileAsync(filePath, ct);
    }
}
