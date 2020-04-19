namespace MAAV.DataContracts
{
    public class BranchActionRequest
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Commit { get; set; }
        public string Message { get; set; }
        public string PreReleaseLabel { get; set; }
        public string BuildLabel { get; set; }
    }
}
