using System;

namespace Gunde.Exceptions
{
    public class GundeAssertionFailedException : Exception
    {
        public GundeAssertionFailedException(string message) : base(message)
        {
        }
        
        public GundeAssertionFailedException(string message, params object[] args) : base(string.Format(message, args))
        {
        }
        
        public GundeAssertionFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}