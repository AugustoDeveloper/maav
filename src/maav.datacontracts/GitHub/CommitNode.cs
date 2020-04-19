using System;
using System.Collections.Generic;
using System.Text;

namespace MAAV.DataContracts.GitHub
{
    public class CommitNode
    {
        /// <summary>
        /// Commit Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Commit message
        /// </summary>
        public string Message { get; set; }
    }

}
