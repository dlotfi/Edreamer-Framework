// Based on Orchard CMS

using System;

namespace Edreamer.Framework.Logging
{
    public interface ILogger
    {
        bool IsEnabled(LogLevel level);
        void Log(LogLevel level, Exception exception, string message, params object[] args);
    }
}
