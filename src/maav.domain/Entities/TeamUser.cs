using System;
using System.Collections.Generic;
using System.Text;

namespace MAAV.Domain.Entities
{
    public class TeamUser
    {
        public TeamUser() { }

        public TeamUser(User user)
        {
            this.Username = user.Username;
            this.CreatedAt = user.CreatedAt;
            this.Id = user.Id;
            this.OrganisationId = this.OrganisationId;
        }

        public string Id { get; set; }
        public string Username { get; set; }
        public string OrganisationId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
