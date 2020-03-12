using System;
using System.Collections.Generic;

namespace MAAV.Domain.Entities
{
    public class Application : IEntity
    {
        public object Id { get; set; }
        public string Name { get; set; }
        public string TeamName { get; set; }
        public string OrganisationName { get; set; }
        public ScheMapVersion ScheMap { get; set; }
        public Version InitialVersion { get; set; }
        public List<BranchVersion> BranchVersions { get; set; } = new List<BranchVersion>();
    }
}