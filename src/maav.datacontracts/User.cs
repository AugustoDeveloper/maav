using System;
using System.Collections.Generic;

namespace MAAV.DataContracts
{
    public class User : TeamUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public List<TeamPermission> TeamsPermissions { get; set; } = new List<TeamPermission>();
        public string[] Roles { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}