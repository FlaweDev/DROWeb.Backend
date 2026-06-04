namespace DROWeb.Domain.Models;

public class ExternalAuth
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string ProviderId { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public DateTime CreatedAt { get; set; }
}
