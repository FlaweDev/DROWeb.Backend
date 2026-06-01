using DROWeb.Domain;
using FluentValidation;

namespace DROWeb.WebAPI.Endpoints.Players.Create;

public record CreatePlayerRequest(string Username);

public record CreatePlayerResponse(Guid Id);

public class Validator : FastEndpoints.Validator<CreatePlayerRequest>
{
    public Validator()
    {
        RuleFor(x => x.Username).NotEmpty().MaximumLength(Player.MaxUsernameLength);
    }
}
