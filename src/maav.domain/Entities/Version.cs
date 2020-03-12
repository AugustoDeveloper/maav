using System;

namespace MAAV.Domain.Entities
{
    public class Version : IEntity
    {
        public long? Major { get; set; }
        public long? Minor { get; set; }
        public long? Patch { get; set; }
        public string Label { get; set; }

        public string FormatVersion(string format)
        {
            var major = $"{Major}";
            var minor = $"{Minor}";
            var patch = $"{Patch}";
            return format.Replace("{Major}", major).Replace("{Minor}", minor).Replace("{Patch}", patch).Replace("{Label}", Label);
        }
    }
}