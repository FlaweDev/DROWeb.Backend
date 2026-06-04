using DROWeb.Application.Interfaces;
using DROWeb.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DROWeb.Persistence;

public class UsersDbContext : DbContext, IUsersDbContext
{
    public DbSet<User> Users { get; set; } = default!;
    public DbSet<ExternalAuth> ExternalAuths { get; set; } = default!;

    public UsersDbContext(DbContextOptions<UsersDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Username).IsRequired().HasMaxLength(100);
            entity.Property(u => u.CreatedAt);
        });

        modelBuilder.Entity<ExternalAuth>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Provider).IsRequired();
            entity.Property(e => e.ProviderId).IsRequired();
            entity.Property(e => e.CreatedAt);

            entity.HasOne<User>()
                .WithMany(u => u.ExternalAuths)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        base.OnModelCreating(modelBuilder);
    }
}
