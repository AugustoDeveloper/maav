using System;

namespace MAAV.DataContracts
{
    public class Authentication
    {
        public string AccessToken { get; set; }
        public DateTime Expiration { get; set; }
        public User User { get; set; }
        public string OrganisationId { get; set; }
    }
}