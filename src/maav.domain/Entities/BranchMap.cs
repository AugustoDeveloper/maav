using System;

namespace MAAV.Domain.Entities
{
    public class BranchMap : IEntity
    {
        public string Name { get; set; }
        public string BranchPattern { get; set; }
        public bool AllowBumpMajor { get; set; }
        public IncrementMode Increment { get; set; }
        public LabelDescription Label { get; set; }
        public string SuffixFormat { get; set; }
        public bool IsKeyBranch { get; set; }
        public string InheritedFrom { get; set; }
    }
}