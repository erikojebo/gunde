using System;
using Gunde.Exceptions;

namespace Gunde.UI.Exceptions
{
    public class GundeConversionException : GundeException
    {
        public GundeConversionException(string message) : base(message)
        {
        }

        public GundeConversionException(string message, params object[] args) : base(message, args)
        {
        }

        public GundeConversionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}