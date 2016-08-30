namespace Edreamer.Framework.Injection
{
    public interface IPropertyInjector
    {
        InjectionResult Inject<TInjector>(object injectorParameter, params object[] sources) where TInjector : IValueInjector;
        InjectionResult Inject<TInjector>(object injectorParameter) where TInjector : INoSourceValueInjector;
        InjectionResult DoNotInject();
    }
}
