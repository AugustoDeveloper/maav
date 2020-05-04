using System;
using System.Collections.Generic;

namespace MAAV.Domain.Entities
{
    public class User : TeamUser, IEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public List<TeamPermission> TeamsPermissions { get; set; }  = new List<TeamPermission>();
        public string[] OrganisationRoles { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
    }
}