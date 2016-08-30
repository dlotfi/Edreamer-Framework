using System;
using JetBrains.Annotations;

namespace Edreamer.Framework.UI.Resources
{
    public interface IContentLocator
    {
        string GetContentUrl(string moduleName, [PathReference]string contentPath);
        string GetContentUrl(Type type, [PathReference]string contentPath);
    }
}