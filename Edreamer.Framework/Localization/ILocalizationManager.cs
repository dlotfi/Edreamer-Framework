namespace Edreamer.Framework.Localization
{
    public interface ILocalizationManager
    {
        string GetLocalizedString(string scope, string text, string cultureName);
    }
}
