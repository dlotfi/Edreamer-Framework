using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Registration;

namespace Edreamer.Framework.Composition
{
    public static class PartBuilderExtensions
    {
        public static PartBuilder SetScopeToApplication(this PartBuilder partBuilder)
        {
            return partBuilder.AddMetadata(CompositionConstants.ApplicationScopeMetadataName, true);
        }

        public static PartBuilder SetAsSingleton(this PartBuilder partBuilder)
        {
            return partBuilder.SetScopeToApplication().SetCreationPolicy(CreationPolicy.Shared);
        }

        public static PartBuilder SetPriority(this PartBuilder partBuilder, int priority)
        {
            return partBuilder.AddMetadata(CompositionConstants.PriorityMetadataName, priority);
        }
    }
}
