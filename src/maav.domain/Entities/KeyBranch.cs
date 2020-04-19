namespace MAAV.Domain.Entities
{
    public class KeyBranch : IEntity
    {
        public string Name { get; set; }
        public string BranchPattern { get; set; }
        public bool IsEnabled { get; set; } = true;
        public string FormatVersion { get; set; }
    }
}