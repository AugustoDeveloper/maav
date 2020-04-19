using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MAAV.DataContracts.GitHub
{
    public class PullRequestNode
    {
        public long Id { get; set; }
        public string State { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string MergeCommitSha { get; set; }
        public BranchNode Head { get; set; }
        public BranchNode Base { get; set; }
        public bool Merged { get; set; }
    }
}
