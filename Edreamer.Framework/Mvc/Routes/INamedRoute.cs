namespace Edreamer.Framework.Mvc.Routes
{
    public interface IAnyNamedRoute
    {
        string Name { get; }
    }

    public interface INamedRoute : IAnyNamedRoute
    {
        string Get();
    }

    public interface INamedRoute<in TModel> : IAnyNamedRoute
    {
        string Get(TModel model);
    }
}
