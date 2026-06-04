using System;
using System.Collections.Generic;
using System.Text;

namespace DROWeb.Persistence
{
    public class DbInitializer
    {
        public static void Initialize(UsersDbContext context) { 
            context.Database.EnsureCreated();
        }
    }
}
