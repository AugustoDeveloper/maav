namespace MAAV.DataContracts.GitHub
{
    public class GitHubWebHookRequest
    {
        public string Event { get; set; }
        public string ShaKey { get; set; }
        public string Data { get; set; }
    }
}
