using FastEndpoints;

namespace DROWeb.WebAPI.Endpoints.Pages.Admin;

public class AdminPageEndpoint : EndpointWithoutRequest
{
    private readonly IWebHostEnvironment _env;

    public AdminPageEndpoint(IWebHostEnvironment env)
    {
        _env = env;
    }

    public override void Configure()
    {
        Get("/admin");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var filePath = Path.Combine(_env.WebRootPath, "admin.html");

        if (!File.Exists(filePath))
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        HttpContext.Response.ContentType = "text/html; charset=utf-8";
        await HttpContext.Response.SendFileAsync(filePath, ct);
    }
}
