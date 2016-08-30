// Based on Orchard CMS

using System;

namespace Edreamer.Framework.Logging
{
    public interface ILoggerFactory
    {
        ILogger CreateLogger(Type type);
    }
}