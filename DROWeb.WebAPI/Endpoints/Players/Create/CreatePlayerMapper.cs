using DROWeb.Domain;
using Riok.Mapperly.Abstractions;

namespace DROWeb.WebAPI.Endpoints.Players.Create;

[Mapper]
public partial class CreatePlayerMapper
{
    public partial CreatePlayerResponse ToResponse(Player player);
}