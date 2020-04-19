using System;
using System.Collections.Generic;
using System.Text;

namespace MAAV.DataContracts
{
    public class TeamApplication
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string TeamId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
