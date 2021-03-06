﻿using System.Collections.Generic;
using Edreamer.Framework.Logging;

namespace Edreamer.Framework.UI.Notification
{
    public class Notifier : INotifier
    {
        private readonly IList<NotifyEntry> _entries;

        public Notifier()
        {
            _entries = new List<NotifyEntry>();
        }

        public ILogger Logger { get; set; }

        public void Add(NotifyType type, string message)
        {
            Logger.Information("Notification {0} message: {1}", type, message);
            _entries.Add(new NotifyEntry { Type = type, Message = message });
        }

        public IEnumerable<NotifyEntry> List()
        {
            return _entries;
        }
    }
}