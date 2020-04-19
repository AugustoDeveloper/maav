using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MAAV.DataContracts.GitHub
{
    public class BranchNode
    {
        public string Label { get; set; }
        public string Ref { get; set; }
        public string Sha { get; set; }
    }
}
