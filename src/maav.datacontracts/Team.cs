using System;
using System.Collections.Generic;

namespace MAAV.DataContracts
{
    public class Team
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public TeamUser[] Users { get; set; }
    }
}