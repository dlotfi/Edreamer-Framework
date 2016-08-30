namespace Edreamer.Framework.Mvc.WebParts
{
    public interface IPartialStore
    {
        string GetPartialViewPath(string area, string viewPath);
    }
}
