using System;
using System.Collections.Generic;

namespace Edreamer.Framework.UI.MetaData
{
    public class MetaEntry
    {
        public MetaEntry()
        {
            Attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public string Name { get; set; }
        public string Content { get; set; }
        public string HttpEquiv { get; set; }
        public string Charset { get; set; }
        public IDictionary<string, string> Attributes { get; private set; }

        public MetaEntry AddAttribute(string name, string value, bool replaceExisting = false)
        {
            if (!Attributes.ContainsKey(name))
            {
                Attributes.Add(name, value);
            }
            else if (replaceExisting)
            {
                Attributes[name] = value;
            }
            return this;
        }
    }
}
