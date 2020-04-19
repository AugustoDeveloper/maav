namespace MAAV.DataContracts
{
    public class KeyBranch
    {
        public string Name { get; set; }
        public string BranchPattern { get; set; }
        public bool IsEnabled { get; set; } = true;
        public string FormatVersion { get; set; }

    }
}