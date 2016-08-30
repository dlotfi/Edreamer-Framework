// Based on Orchard CMS

using System;

namespace Edreamer.Framework.Context
{
    public interface IWorkContext
    {
        T GetState<T>(string name);
        bool TryGetState<T>(string name, out T value);
        void SetState<T>(string name, T value);
        bool StateExists(string name);
    }
}
