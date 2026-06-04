using DROWeb.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DROWeb.Application.Interfaces;

public interface IUsersDbContext
{
    DbSet<User> Users { get; set; }
    DbSet<ExternalAuth> ExternalAuths { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
