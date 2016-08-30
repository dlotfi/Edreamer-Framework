using System.Collections.Generic;

namespace Edreamer.Framework.Mvc.Layouts
{
    public interface ILayoutRegistrar
    {
        void RegisterLayouts(ICollection<Layout> layouts);
    }
}
