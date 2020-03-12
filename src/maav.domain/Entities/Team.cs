using System;

namespace MAAV.Domain.Entities
{
    public class Team : IEntity
    {
        public object Id { get; set; }
        public string OrganisationName { get; set; }
        public string Name { get; set; }
        public Application[] Applications { get; set; }
        public User[] Users { get; set; }
        public ScheMapVersion ScheMap { get; set; }
    }
}