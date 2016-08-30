namespace Edreamer.Framework.Module
{
    public interface IModuleEventHandler
    {
        void Installed(string moduleName);
        void Uninstalled(string moduleName);
    }
}
