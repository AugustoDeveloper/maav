using System;
using System.Collections.Generic;

namespace MAAV.Domain.Entities
{
    public class Team : IEntity
    {
        public string Id { get; set; }
        public string OrganisationId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<Application> Applications { get; set; } = new List<Application>();
        public List<TeamUser> Users { get; set; } = new List<TeamUser>();
    }
}