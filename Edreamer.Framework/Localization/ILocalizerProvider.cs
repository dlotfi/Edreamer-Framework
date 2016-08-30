namespace Edreamer.Framework.Localization
{
    public interface ILocalizerProvider
    {
        Localizer GetLocalizer(string scope);
        Localizer GetLocalizer(string scope, string currentCulture);
    }
}
