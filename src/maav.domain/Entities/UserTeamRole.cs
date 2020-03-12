using System;
using System.Collections.Generic;

namespace MAAV.Domain.Entities
{
    public class UserTeamRole : IEntity
    {
        public object TeamId { get; set; }
        public string TeamName { get; set; }
        public string[] Roles { get; set; }
    }
}