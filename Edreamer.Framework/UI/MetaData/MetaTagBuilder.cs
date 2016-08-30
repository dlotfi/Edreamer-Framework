using System;
using System.Web;
using System.Web.Mvc;

namespace Edreamer.Framework.UI.MetaData
{
    public class MetaTagBuilder: IMetaTagBuilder
    {
        public IHtmlString BuildMetaTag(MetaEntry meta)
        {
            var builder = new TagBuilder("meta");
            SetAttributeIfNotEmpty(builder, "name", meta.Name);
            SetAttributeIfNotEmpty(builder, "content", meta.Content);
            SetAttributeIfNotEmpty(builder, "http-equiv", meta.HttpEquiv);
            SetAttributeIfNotEmpty(builder, "charset", meta.Charset);
            foreach (var attribute in meta.Attributes)
            {
                SetAttributeIfNotEmpty(builder, attribute.Key, attribute.Value);
            }
            return new HtmlString(builder.ToString(TagRenderMode.SelfClosing));
        }

        private void SetAttributeIfNotEmpty(TagBuilder builder, string name, string value)
        {
            if (!String.IsNullOrEmpty(value))
                builder.MergeAttribute(name, value);
        }
    }
}