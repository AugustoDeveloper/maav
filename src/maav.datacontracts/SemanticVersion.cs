using System;

namespace MAAV.DataContracts
{
    public class SemanticVersion
    {
        public long? Major { get; set; }
        public long? Minor { get; set; }
        public long? Patch { get; set; }
        public string PreRelease { get; set; }
        public string Build { get; set; }
    }
}