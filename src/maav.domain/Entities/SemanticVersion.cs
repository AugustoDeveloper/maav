using System;

namespace MAAV.Domain.Entities
{
    public class SemanticVersion : IEntity
    {
        public long? Major { get; set; }
        public long? Minor { get; set; }
        public long? Patch { get; set; }
        public string PreRelease { get; set; }
        public string Build { get; set; }

        public override string ToString()
        {
            var preRelease = string.IsNullOrEmpty(PreRelease) ? $"-{PreRelease}" : "";
            var build = string.IsNullOrEmpty(Build) ? $"+{Build}" : "";
            return $"{Major}.{Minor}.{Patch}{preRelease}{build}";
        }
    }
}