using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Mvc.ViewEngine
{
    public abstract class ViewStartPage : System.Web.Mvc.ViewStartPage
    {
        public override string Layout
        {
            get
            {
                return base.Layout;
            }
            set
            {
                // Default implementation check for layout existance before assigning it to child page
                if (value != null && value.EnclosedBy("<", ">") && (ChildPage is IViewPage || ChildPage is ViewStartPage))
                {
                    ChildPage.Layout = value;
                }
                else
                {
                    base.Layout = value;
                }
            }
        }

        public string Template
        {
            get
            {
                var childPage = ChildPage as IViewPage;
                return childPage != null ? childPage.Template : null;
            }
            set
            {
                var childPage = ChildPage as IViewPage;
                if (childPage != null)
                {
                    childPage.Template = value;
                }
            }
        }
    }


}