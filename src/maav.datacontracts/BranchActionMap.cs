namespace MAAV.DataContracts
{
    public class BranchActionMap
    {
        public string Name { get; set; }
        public string BranchPattern { get; set; }
        public bool AllowBumpMajor { get; set; }
        public string BumpMajorText { get; set; }
        public IncrementMode Increment { get; set; }
        public KeyBranch InheritedFrom { get; set; }
    }
}