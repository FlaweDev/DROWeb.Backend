namespace DROWeb.Domain.Models;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public ICollection<ExternalAuth> ExternalAuths { get; set; } = new List<ExternalAuth>();
    public DateTime CreatedAt { get; set; }
}
