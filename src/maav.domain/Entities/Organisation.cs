using System;

namespace MAAV.Domain.Entities
{
    public class Organisation : IEntity
    {
        public string Name { get; set; }
        public Team[] Teams { get; set; }
        public ScheMapVersion ScheMap { get; set; }
    }
}