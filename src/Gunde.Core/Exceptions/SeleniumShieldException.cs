using System;

namespace Gunde.Exceptions
{
    public class GundeException : Exception
    {
        public GundeException(string message) : base(message)
        {
        }
        
        public GundeException(string message, params object[] args) : base(string.Format(message, args))
        {
        }
        
        public GundeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}