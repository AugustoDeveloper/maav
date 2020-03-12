using System;
using System.Collections.Generic;

namespace MAAV.Domain.Entities
{
    public class User : IEntity
    {
        public object Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OrganisationName { get; set; }
        public List<UserTeamRole> TeamRoles { get; set; }  = new List<UserTeamRole>();
        public string[] OrganisationRoles { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}