using DROWeb.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DROWeb.Application.Interfaces
{
    public interface IPlayersDbContext
    {
        DbSet<Player> Players { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
