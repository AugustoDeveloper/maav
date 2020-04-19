using System;
using System.Collections.Generic;
using System.Text;

namespace MAAV.Domain.Entities
{
    public class GithubEventResult : IEntity
    {
        public string Id { get; set; }
        public long PullRequestId { get; set; }
        public string FromBranch { get; set; }
        public string ToBranch { get; set; }
        public string PushCommit { get; set; }
        public string CommitMessage { get; set; }
        public string Status { get; set; }
    }
}
