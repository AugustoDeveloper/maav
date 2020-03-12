using System;

namespace MAAV.Application.Exceptions
{
    public class NameAlreadyUsedException : Exception
    {
        public NameAlreadyUsedException(string name) : base($"The {name} used for another org.") { }
    }
}
