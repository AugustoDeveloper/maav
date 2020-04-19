using System.Collections.Generic;

namespace MAAV.DataContracts
{
    public class KeyBranchVersionHistory
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
