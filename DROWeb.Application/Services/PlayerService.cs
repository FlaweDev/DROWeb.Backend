using DROWeb.Application.Interfaces;
using DROWeb.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace DROWeb.Application.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayersDbContext _dbContext;

        public PlayerService(IPlayersDbContext dbContext) => _dbContext = dbContext;

        public async Task<Player> CreateAsync(string username, CancellationToken ct)
        {
            if (username == null) 
                throw new ArgumentNullException("Username is missing");
            if (string.IsNullOrEmpty(username)) 
                throw new ArgumentNullException("Username is empty");

            username = username.Trim();

            if (_dbContext.Players.Any(x => x.Username == username))
                throw new Exception($"Player with username \"${username}\" already created");

            var player = new Player()
            {
                Id = Guid.NewGuid(),
                Username = username,
                CreationDate = DateTime.Now,
                XP = 0
            };

            _dbContext.Players.Add(player);

            await _dbContext.SaveChangesAsync(ct);

            return player;
        }
    }
}
