using System;
using System.Collections.Generic;
using System.Text;

namespace MAAV.DataContracts
{
    public class KeyBranchVersion
    {
        public string Id { get; set; }
        public string PreviousId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string FormatVersion { get; set; }
        public SemanticVersion Version { get; set; }
        public BranchActionRequest Request { get; set; }
    }
}
