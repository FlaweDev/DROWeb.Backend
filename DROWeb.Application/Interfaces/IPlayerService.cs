using DROWeb.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace DROWeb.Application.Interfaces
{
    public interface IPlayerService
    {
        Task<Player> CreateAsync(string username, CancellationToken ct);
    }
}
