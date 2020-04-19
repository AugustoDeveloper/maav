using System;
using System.Collections.Generic;
using System.Text;

namespace MAAV.Domain.Entities
{
    public class KeyBranchVersionHistory : IEntity
    {
        public string Id { get; set; }
        public string OrganisationId { get; set; }
        public string TeamId { get; set; }
        public string ApplicationId { get; set; }
        public string KeyBranchName { get; set; }
        public string LastHistoryId { get; set; }
        public List<KeyBranchVersion> VersionHistory { get; set; } = new List<KeyBranchVersion>();
    }
}
