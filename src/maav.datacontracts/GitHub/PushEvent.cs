using System;
using System.Collections.Generic;
using System.Text;

namespace MAAV.DataContracts.GitHub
{
    public class PushEvent : IGithubEvent
    {
        public string OrganisationId { get; set; }
        public string TeamId { get; set; }
        public string ApplicationId { get; set; }
        /// <summary>
        /// Full Branch Name  /ref/heads/master
        /// </summary>
        public string Ref { get; set; }

        /// <summary>
        /// Before Commit 
        /// </summary>
        public string Before { get; set; }

        /// <summary>
        /// After Commit (HEAD)
        /// </summary>
        public string After { get; set; }
        /// <summary>
        /// All commits related
        /// </summary>
        public CommitNode[] Commits { get; set; }

        /// <summary>
        /// Head Commit Info
        /// </summary>
        public CommitNode HeadCommit { get; set; }
    }
}
