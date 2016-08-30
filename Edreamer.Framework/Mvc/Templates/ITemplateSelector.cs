namespace Edreamer.Framework.Mvc.Templates
{
    public interface ITemplateSelector
    {
        string GetTemplatePath(string templateName, string templateContext);
    }
}
