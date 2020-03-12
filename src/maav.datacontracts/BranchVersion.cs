namespace MAAV.DataContracts
{
    public class BranchVersion
    {
        public string BranchMapName { get; set; }
        public bool IsEnabled { get; set; } = true;
        public Version Version { get; set; }
    }
}