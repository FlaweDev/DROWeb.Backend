
using DROWeb.Application.Interfaces;
using FastEndpoints;

namespace DROWeb.WebAPI.Endpoints.Players.Create;

public class CreatePlayerEndpoint : Endpoint<CreatePlayerRequest, CreatePlayerResponse>
{
    private readonly IPlayerService _playerService;
    private readonly CreatePlayerMapper _mapper;

    public CreatePlayerEndpoint(IPlayerService playerService)
    {
        _playerService = playerService;
        _mapper = new CreatePlayerMapper();
    }

    public override void Configure()
    {
        Post("/api/player/create");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreatePlayerRequest req, CancellationToken ct)
    {
        var createdPlayer = await _playerService.CreateAsync(
            req.Username,
            ct);

        var response = _mapper.ToResponse(createdPlayer);

        await Send.OkAsync(response, ct);
    }
}