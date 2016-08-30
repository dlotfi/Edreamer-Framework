using System.Collections.Generic;

namespace Edreamer.Framework.Module
{
    public class FrameworkModule : Module
    {
        public override IEnumerable<string> Namespaces
        {
            get { return new[] { "Edreamer.Framework.*" }; }
        }

        public override string Name
        {
            get { return "Framework"; }
        }
    }
}
