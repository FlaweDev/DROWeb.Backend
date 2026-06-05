using DROWeb.Application.Interfaces;
using DROWeb.Domain.Models;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace DROWeb.WebAPI.Endpoints.Users.Avatar;

public class AvatarEndpoint : Endpoint<AvatarRequest, AvatarResponse>
{
    private readonly IUsersDbContext _dbContext;
    private readonly IAvatarService _avatarService;

    public AvatarEndpoint(IUsersDbContext dbContext, IAvatarService avatarService)
    {
        _dbContext = dbContext;
        _avatarService = avatarService;
    }

    public override void Configure()
    {
        Get("/users/{Id}/avatar");
        AllowAnonymous();
        Description(x => x.WithName("GetUserAvatar"));
    }

    public override async Task HandleAsync(AvatarRequest req, CancellationToken ct)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == req.Id, ct);
        if (user == null)
        {
            await Send.NotFoundAsync();
            return;
        }

        var avatarUrl = await _avatarService.GetAvatarUrl(req.Id, ct);
        await Send.OkAsync(new AvatarResponse(avatarUrl));
    }
}

public record AvatarRequest(Guid Id);
public record AvatarResponse(string AvatarUrl);
