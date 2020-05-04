using System;
using System.Collections.Generic;

namespace MAAV.Domain.Entities
{
    public class TeamPermission : IEntity
    {
        public string TeamCode { get; set; }
        public bool IsReader { get; set; }
        public bool IsWriter { get; set; }
        public bool IsOwner { get; set; }
    }
}