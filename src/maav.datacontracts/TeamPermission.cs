using System;
using System.Collections.Generic;

namespace MAAV.DataContracts
{
    public class TeamPermission
    {
        public string TeamId { get; set; }
        public bool IsReader { get; set; }
        public bool IsWriter { get; set; }
        public bool IsOwner { get; set; }
    }
}