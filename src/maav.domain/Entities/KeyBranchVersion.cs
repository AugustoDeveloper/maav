using System;
using System.Collections.Generic;
using System.Text;

namespace MAAV.Domain.Entities
{
    public class KeyBranchVersion : IEntity
    {
        public string Id { get; set; }
        public string PreviousId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string FormatVersion { get; set; }
        public SemanticVersion Version { get; set; }
        public BranchActionRequest Request { get; set; }
    }
}
