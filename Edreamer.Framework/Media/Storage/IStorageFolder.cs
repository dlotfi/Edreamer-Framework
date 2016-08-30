// Based on Orchard CMS

using System;

namespace Edreamer.Framework.Media.Storage
{
    public interface IStorageFolder
    {
        string GetPath();
        string GetName();
        long GetSize();
        DateTime GetLastUpdated();
        IStorageFolder GetParent();
    }
}