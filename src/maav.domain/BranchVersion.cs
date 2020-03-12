namespace MAAV.Domain.Entities
{
    public class BranchVersion : IEntity
    {
        public string BranchMapName { get; set; }
        public bool IsEnabled { get; set; } = true;
        public Version Version { get; set; }
    }
}