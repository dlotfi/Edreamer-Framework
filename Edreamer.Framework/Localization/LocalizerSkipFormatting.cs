namespace Edreamer.Framework.Localization
{
    /// <summary>
    /// Passing an instance of this class to localizer makes it return the translated string without formatting it.
    /// </summary>
    public sealed class LocalizerFormatting
    {
        private LocalizerFormatting()
        {
        }

        public static readonly LocalizerFormatting Skip = new LocalizerFormatting();
    }
}
