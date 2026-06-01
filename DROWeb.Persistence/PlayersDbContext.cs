using DROWeb.Application.Interfaces;
using DROWeb.Domain;
using DROWeb.Persistence.EntityTypeConfiguration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DROWeb.Persistence
{
    public class PlayersDbContext : DbContext, IPlayersDbContext
    {
        public DbSet<Player> Players { get; set; }
        public PlayersDbContext(DbContextOptions<PlayersDbContext> options) 
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PlayerConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
