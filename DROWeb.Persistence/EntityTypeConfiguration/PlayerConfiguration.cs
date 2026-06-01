using DROWeb.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DROWeb.Persistence.EntityTypeConfiguration
{
    public class PlayerConfiguration : IEntityTypeConfiguration<Player>
    {
        public void Configure(EntityTypeBuilder<Player> builder)
        {
            builder.HasKey(p => p.Id);
            builder.HasIndex(p => p.Id).IsUnique();
            builder.Property(p=> p.Username).IsRequired().HasMaxLength(Player.MaxUsernameLength);
            builder.Property(p => p.XP).HasDefaultValue(0);
        }
    }
}
