using System;

namespace Edreamer.Framework.Logging
{
    public class NullLogger : ILogger
    {
        public bool IsEnabled(LogLevel level)
        {
            return false;
        }

        public void Log(LogLevel level, Exception exception, string format, params object[] args)
        {
        }
    }
}