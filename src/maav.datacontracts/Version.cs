using System;

namespace MAAV.DataContracts
{
    public class Version
    {
        public long? Major { get; set; }
        public long? Minor { get; set; }
        public long? Patch { get; set; }
        public string Label { get; set; }

        public Version() { }
        public Version(string version)
        {
            var versionArray = version.Split('.', 3);
            Major = Convert.ToInt64(versionArray[0]);
            Minor = versionArray.Length > 1 && !string.IsNullOrWhiteSpace(versionArray[1]) ? Convert.ToInt64(versionArray[1]) : new long?();
            Patch = versionArray.Length > 2 && !string.IsNullOrWhiteSpace(versionArray[2]) ? Convert.ToInt64(versionArray[2]) : new long?();
        }
    }
}