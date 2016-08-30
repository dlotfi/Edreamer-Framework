namespace Edreamer.Framework.Mvc.Routes
{
    public abstract class VoidNamedRouteBase
    {
        public string Name
        {
            get { return GetType().FullName; }
        }
    }

    public class VoidNamedRoute : VoidNamedRouteBase, INamedRoute
    {
        public virtual string Get()
        {
            return "#";
        }
    }

    public class VoidNamedRoute<TModel> : VoidNamedRouteBase, INamedRoute<TModel>
    {
        public virtual string Get(TModel model)
        {
            return "#";
        }
    }

    public class VoidNamedRouteWithOptionalParameter<TModel> : VoidNamedRoute<TModel>, INamedRoute
    {
        public virtual string Get()
        {
            return "#";
        }
    }
}