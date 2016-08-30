using System;

namespace Edreamer.Framework.Logging
{
    public class LoggerFactory : ILoggerFactory
    {
        private static readonly ILogger NullLoggerInstance = new NullLogger();

        public ILogger CreateLogger(Type type)
        {
            return NullLoggerInstance;
        }
    }
}
