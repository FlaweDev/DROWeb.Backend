using System;
using System.Collections.Generic;
using System.Text;

namespace DROWeb.Domain
{
    public class Player
    {
        public const int MaxUsernameLength = 28;

        public Guid Id { get; set; }
        //public DateTime CreationDate { get; set; }
        public string Username { get; set; }
        public int XP {  get; set; }

    }
}
