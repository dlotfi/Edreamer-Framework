using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Edreamer.Framework.Context;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Mvc.Extensions;

namespace Edreamer.Framework.UI.Resources
{
    public class ResourceTagBuilder : IResourceTagBuilder
    {
        private readonly IWorkContextAccessor _workContextAccessor;

        private static readonly IDictionary<string, string> ResourceTypeTagNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "script", "script" },
            { "stylesheet", "link" }
        };
        private static readonly IDictionary<string, string> TagPathAttributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "script", "src" },
            { "link", "href" }
        };
        private static readonly IDictionary<string, IDictionary<string, string>> ResourceAttributes = new Dictionary<string, IDictionary<string, string>>(StringComparer.OrdinalIgnoreCase)
        {
            { "script", new Dictionary<string, string> { {"type", "text/javascript"} } },
            { "stylesheet", new Dictionary<string, string> { {"type", "text/css"}, {"rel", "stylesheet"} } }
        };
        private static readonly IDictionary<string, bool> TagSelfClosing = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase)
        {
            { "script", false },
            { "link", true }
        };

        public ResourceTagBuilder(IWorkContextAccessor workContextAccessor)
        {
            Throw.IfArgumentNull(workContextAccessor, "workContextAccessor");
            _workContextAccessor = workContextAccessor;
        }

        public virtual IHtmlString BuildResourceTag(string type, string path, IDictionary<string, string> attributes, string condition, bool absoluteUrl)
        {
            Throw.IfArgumentNullOrEmpty(type, "type");
            Throw.IfArgumentNullOrEmpty(path, "path");
            Throw.If(!ResourceTypeTagNames.ContainsKey(type))
                .AnArgumentException("The type {0} is not registered.".FormatWith(type), "type");

            var tagBuilder = new TagBuilder(ResourceTypeTagNames[type]);
            var tagRenderMode = TagSelfClosing[tagBuilder.TagName] ? TagRenderMode.SelfClosing : TagRenderMode.Normal;
            if (!Uri.IsWellFormedUriString(path, UriKind.Absolute))
            {
                path = VirtualPathUtility.ToAbsolute(path);
                var httpContext = _workContextAccessor.Context.CurrentHttpContext();
                Throw.If(httpContext == null && absoluteUrl)
                    .A<InvalidOperationException>("Cannot build resource tag with absolute url when not in a web request.");
                if (absoluteUrl)
                {
                    var urlHelper = new UrlHelper(httpContext.Request.RequestContext);
                    path = urlHelper.ConvertToAbsoluteUrl(path);
                }
            }
            tagBuilder.MergeAttributes(ResourceAttributes[type], true);
            tagBuilder.MergeAttribute(TagPathAttributes[tagBuilder.TagName], path, true);
            tagBuilder.MergeAttributes(CollectionHelpers.EmptyIfNull(attributes));
            var stringBuilder = new StringBuilder();
            if (!string.IsNullOrEmpty(condition))
            {
                stringBuilder.AppendLine("<!--[if " + condition + "]>");
            }
            stringBuilder.AppendLine(tagBuilder.ToString(tagRenderMode));
            if (!string.IsNullOrEmpty(condition))
            {
                stringBuilder.AppendLine("<![endif]-->");
            }
            return new HtmlString(stringBuilder.ToString());
        }

        public static void RegisterResourceTag(string type, string tagName, string pathAttribute, IDictionary<string, string> resourceAttributes, bool selfClosingTag)
        {
            Throw.IfArgumentNullOrEmpty(type, "type");
            Throw.IfArgumentNullOrEmpty(tagName, "tagName");
            Throw.If(ResourceAttributes.ContainsKey(type))
                .AnArgumentException("The type {0} is already defined.".FormatWith(type), "type");
            if (!TagPathAttributes.ContainsKey(tagName))
            {
                Throw.IfArgumentNullOrEmpty(pathAttribute, "pathAttribute");
            }
            else
            {
                Throw.If(!String.IsNullOrEmpty(pathAttribute) && !pathAttribute.EqualsIgnoreCase(TagPathAttributes[tagName]))
                   .AnArgumentException("The tag {0} has been registered with another path attribute".FormatWith(tagName), "pathAttribute");
                Throw.If(selfClosingTag != TagSelfClosing[tagName])
                    .AnArgumentException("The tag {0} has been registered as{1} self closing tag.".FormatWith(tagName, TagSelfClosing[tagName] ? "" : " not"), "selfClosingTag");
            }
            ResourceTypeTagNames[type] = tagName;
            ResourceAttributes[type] = CollectionHelpers.EmptyIfNull(resourceAttributes);
            if (!String.IsNullOrEmpty(pathAttribute)) TagPathAttributes[tagName] = pathAttribute;
            TagSelfClosing[tagName] = selfClosingTag;
        }
    }
}
