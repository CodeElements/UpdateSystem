using System;
using System.Collections.Generic;
using System.Threading;

namespace CodeElements.UpdateSystem.Windows.Patcher.Utilities
{
    public static class Retry
    {
        public static void Do(Action action, TimeSpan retryInterval, int maxAttemptCount = 3, Logger logger = null)
        {
            Do<object>(() =>
            {
                action();
                return null;
            }, retryInterval, maxAttemptCount, logger);
        }

        public static T Do<T>(Func<T> action, TimeSpan retryInterval, int maxAttemptCount = 3, Logger logger = null)
        {
            var exceptions = new List<Exception>();

            for (var attempted = 0; attempted < maxAttemptCount; attempted++)
                try
                {
                    if (attempted > 0)
                        Thread.Sleep(retryInterval);
                    return action();
                }
                catch (Exception ex)
                {
                    if (logger != null)
                    {
                        var message = $"Attempt #{attempted + 1} failed: {ex.Message}";
                        if (attempted == maxAttemptCount - 1) logger.Error(message);
                        else logger.Warning(message);
                    }

                    exceptions.Add(ex);
                }
            throw new AggregateException(exceptions);
        }
    }
}