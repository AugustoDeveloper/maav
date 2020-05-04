using System;
using System.Collections.Generic;
using System.Text;

namespace MAAV.Domain.Entities
{
    public class TeamApplication : IEntity
    {
        public TeamApplication() { }
        public TeamApplication(Application app)
        {
            this.Id = app.Id;
            this.Name = app.Name;
            this.CreatedAt = app.CreatedAt;
            this.TeamCode = app.TeamCode;
            this.OrganisationId = app.OrganisationId;
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TeamCode { get; set; }
        public string OrganisationId { get; set; }
    }
}
