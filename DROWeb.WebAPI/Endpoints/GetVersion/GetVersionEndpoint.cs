using DROWeb.Application.Interfaces;
using FastEndpoints;

namespace DROWeb.WebAPI.Endpoints.GetVersion
{
    public class GetVersionEndpoint : Endpoint<GetVersionRequest, CreatePlayerResponse>
    {
        public override void Configure()
        {
            Post("/api/version");
            AllowAnonymous();
            Description(x => x.WithName("GetVersion"));
        }

        public override async Task HandleAsync(GetVersionRequest req, CancellationToken ct)
        {
            var response = new CreatePlayerResponse("1.0.0 ");

            await Send.OkAsync(response, ct);
        }
    }
}
