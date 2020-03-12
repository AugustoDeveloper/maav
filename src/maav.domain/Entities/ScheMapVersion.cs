using System;

namespace MAAV.Domain.Entities
{
    public class ScheMapVersion : IEntity
    {
        public string Format { get; set; }
        public BranchMap[] Branches { get; set; }
    }
}