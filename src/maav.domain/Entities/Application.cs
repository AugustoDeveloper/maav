using System;
using System.Collections.Generic;

namespace MAAV.Domain.Entities
{
    public class Application : TeamApplication
    {
        public BranchActionMap[] Branches { get; set; }
        public SemanticVersion InitialVersion { get; set; }
        public List<KeyBranch> KeyBranches { get; set; } = new List<KeyBranch>();
        public List<KeyBranchVersioning> KeyBranchVersionings { get; set; } = new List<KeyBranchVersioning>();
        public string GithubSecretKey { get; set; }
        public bool WebHookEnabled { get; set; }
    }
}