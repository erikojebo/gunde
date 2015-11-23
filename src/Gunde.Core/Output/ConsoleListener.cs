using System;

namespace Gunde.Core.Output
{
    public class ConsoleListener : IResultListener
    {
        public void OutputLine(string message, params object[] formatParams)
        {
            Console.WriteLine(message, formatParams);
        }
    }
}