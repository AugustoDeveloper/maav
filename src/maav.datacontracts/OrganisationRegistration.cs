using System;

namespace MAAV.DataContracts
{
    public class OrganisationRegistration : Organisation
    {
        public User[] AdminUsers { get; set; }
    }
}
