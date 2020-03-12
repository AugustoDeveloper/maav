using System;
using System.Collections.Generic;

namespace MAAV.DataContracts
{
    public class Application
    {
        public string Name { get; set; }
        public ScheMapVersion ScheMap { get; set; }
        public string InitialVersion { get; set; }
        public List<BranchVersion> BranchVersions { get; set; } = new List<BranchVersion>();
    }
}