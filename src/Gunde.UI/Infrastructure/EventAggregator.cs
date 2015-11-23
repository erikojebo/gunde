using System;

namespace Gunde.UI.Infrastructure
{
    public class EventAggregator
    {
        public static event Action<string> TaskExecutionFailed;

        public static void FireTaskExecutionFailed(string message)
        {
            TaskExecutionFailed?.Invoke(message);
        }
    }
}