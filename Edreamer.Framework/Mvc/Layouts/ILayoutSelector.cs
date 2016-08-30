namespace Edreamer.Framework.Mvc.Layouts
{
    public interface ILayoutSelector
    {
        string GetLayoutPath(string layoutName);
    }
}
