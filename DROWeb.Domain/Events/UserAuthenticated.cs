using MediatR;

namespace DROWeb.Domain.Events;

public class UserAuthenticated : INotification
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? ExternalId { get; set; }
    public string Provider { get; set; } = string.Empty;
}
