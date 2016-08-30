using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.WebPages;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Mvc.DeferredRender
{
    public static class DeferredRenderHelper
    {
        public static string ProcessDefferedRenders(string content, IDictionary<string, Func<HelperResult>> deferredRenders)
        {
            if (String.IsNullOrEmpty(content) || deferredRenders == null || !deferredRenders.Any())
            {
                return content;
            }

            var replacedKeysList = new List<string>();
            var result = Regex.Replace(content, "(" + String.Join("|", deferredRenders.Keys.ToArray()) + ")",
                match =>
                {
                    Throw.IfNullOrEmpty(match.Value).AnArgumentException("Deferred render key cannot be empty.", "deferredRenders");
                    var deferredRenderedContent = deferredRenders[match.Value]();
                    Throw.IfNull(deferredRenderedContent).AnArgumentException("Deferred render content cannot be null.", "deferredRenders");
                    var textWriter = new StringWriter();
                    deferredRenderedContent.WriteTo(textWriter);
                    replacedKeysList.Add(match.Value);
                    return textWriter.ToString();
                });
            replacedKeysList.ForEach(x => deferredRenders.Remove(x));
            return result;
        }
    }
}