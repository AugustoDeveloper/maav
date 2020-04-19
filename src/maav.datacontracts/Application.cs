using System;
using System.Collections.Generic;

namespace MAAV.DataContracts
{
    public class Application : TeamApplication
    {
        public BranchActionMap[] Branches { get; set; }
        public SemanticVersion InitialVersion { get; set; }
        public List<KeyBranch> KeyBranches { get; set; } = new List<KeyBranch>();
        public string GithubSecretKey { get; set; }
        public bool WebHookEnabled { get; set; }
    }
}