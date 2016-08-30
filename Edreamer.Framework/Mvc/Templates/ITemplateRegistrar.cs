using System.Collections.Generic;

namespace Edreamer.Framework.Mvc.Templates
{
    public interface ITemplateRegistrar
    {
        string TemplateContext { get; }
        string BaseTemplateContext { get; }

        void RegisterTemplates(ICollection<Template> templates);
    }
}
