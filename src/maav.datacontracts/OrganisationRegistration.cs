using System;

namespace MAAV.DataContracts
{
    public class OrganisationRegistration : Organisation
    {
        public User AdminUser { get; set; }
    }
}
