using System;
using System.Collections.Generic;
using System.Text;

namespace MAAV.Domain.Entities
{
    public class BranchActionRequest : IEntity
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Commit { get; set; }
        public string Message { get; set; }
        public string PreReleaseLabel { get; set; }
        public string BuildLabel { get; set; }
    }
}
