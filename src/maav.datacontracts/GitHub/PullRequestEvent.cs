using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MAAV.DataContracts.GitHub
{
    public class PullRequestEvent : IGithubEvent
    {
        public string OrganisationId { get; set; }
        public string TeamId { get; set; }
        public string ApplicationId { get; set; }
        public string Action { get; set; }
        public PullRequestNode PullRequest { get; set; }
    }

}
