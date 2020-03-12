using System.Collections.Generic;

namespace MAAV.DataContracts
{
    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<UserTeamRole> TeamRoles { get; set; } = new List<UserTeamRole>();
    }
}