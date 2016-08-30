using System.Collections.Generic;

namespace Edreamer.Framework.UI.Notification
{
    public interface INotifier
    {
        void Add(NotifyType type, string message);
        IEnumerable<NotifyEntry> List();
    }
}
