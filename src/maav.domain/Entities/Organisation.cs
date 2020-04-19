using System;

namespace MAAV.Domain.Entities
{
    public class Organisation : IEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Team[] Teams { get; set; }
    }
}