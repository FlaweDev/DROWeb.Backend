using System;
using System.Collections.Generic;
using System.Text;

namespace DROWeb.Persistence
{
    public class DbInitializer
    {
        public static void Initialize(PlayersDbContext context) { 
            context.Database.EnsureCreated();
        }
    }
}
