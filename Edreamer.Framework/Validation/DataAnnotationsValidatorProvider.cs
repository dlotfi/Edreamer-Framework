// Based on ASP.Net Mvc3 source code (DataAnnotationsModelValidatorProvider.cs)

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using Edreamer.Framework.Composition;
using Edreamer.Framework.DataAnnotations;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Localization;

namespace Edreamer.Framework.Validation
{
    // A factory for validators based on ValidationAttribute
    public delegate IValidator DataAnnotationsAttributeAdapterFactory(ObjectMetadata metadata, ValidationAttribute attribute, Localizer localizer);

    // A factory for validators based on IValidatableObject
    public delegate IValidator DataAnnotationsValidatableObjectAdapterFactory(ObjectMetadata metadata, Localizer localizer);

    /// <summary>
    /// An implementation of <see cref="IValidatorProvider"/> which providers validators
    /// for attributes which derive from <see cref="ValidationAttribute"/>. It also provides
    /// a validator for types which implement <see cref="IValidatableObject"/>.
    /// </summary>
    public class DataAnnotationsValidatorProvider : AssociatedValidatorProvider
    {
        #region Static Members
        private static bool _addImplicitRequiredAttributeForValueTypes = true;
        private static ReaderWriterLockSlim _adaptersLock = new ReaderWriterLockSlim();

        // Factories for validation attributes
        private static DataAnnotationsAttributeAdapterFactory _defaultAttributeFactory =
            (metadata, attribute, localizer) => new DataAnnotationsValidatorAdapter(metadata, attribute, localizer);
        private static IDictionary<Type, DataAnnotationsAttributeAdapterFactory> _attributeFactories =
            new Dictionary<Type, DataAnnotationsAttributeAdapterFactory>();

        // Factories for IValidatableObject objects
        private static DataAnnotationsValidatableObjectAdapterFactory _defaultValidatableFactory =
            (metadata, localizer) => new ValidatableObjectAdapter(metadata, localizer);
        private static IDictionary<Type, DataAnnotationsValidatableObjectAdapterFactory> _validatableFactories =
            new Dictionary<Type, DataAnnotationsValidatableObjectAdapterFactory>();

        public static bool AddImplicitRequiredAttributeForValueTypes
        {
            get { return _addImplicitRequiredAttributeForValueTypes; }
            set { _addImplicitRequiredAttributeForValueTypes = value; }
        }
        #endregion

        private readonly ICompositionContainerAccessor _compositionContainerAccessor;
        private readonly ILocalizerProvider _localizerProvider;

        public DataAnnotationsValidatorProvider(ICompositionContainerAccessor compositionContainerAccessor, ILocalizerProvider localizerProvider)
        {
            Throw.IfArgumentNull(compositionContainerAccessor, "compositionContainerAccessor");
            Throw.IfArgumentNull(localizerProvider, "localizerProvider");
            _compositionContainerAccessor = compositionContainerAccessor;
            _localizerProvider = localizerProvider;
        }

        protected override IEnumerable<IValidator> GetValidators(ObjectMetadata metadata, IEnumerable<Attribute> attributes)
        {
            _adaptersLock.EnterReadLock();
            try
            {
                attributes = attributes.ToList();

                // Add an implied [Required] attribute for any non-nullable value type,
                // unless they've configured us not to do that.
                if (AddImplicitRequiredAttributeForValueTypes &&
                        !metadata.Type.AllowsNullValue() &&
                        !attributes.Any(a => a is RequiredAttribute))
                {
                    attributes = attributes.Concat(new[] { new RequiredAttribute() });
                }

                var modelLocalizer = _localizerProvider.GetLocalizer((metadata.ContainerType ?? metadata.Type).FullName);

                // Produce a validator for each of the validation attribute returned by each composite data annotaion attribute we find
                foreach (var compositeAttribute in attributes.OfType<CompositeDataAnnotationsAttribute>())
                {
                    var globalLocalizer = _localizerProvider.GetLocalizer(compositeAttribute.GetType().FullName);
                    foreach (var attribute in CollectionHelpers.EmptyIfNull(compositeAttribute.GetAttributes().OfType<ValidationAttribute>()))
                    {
                        yield return GetDataAnnotationsAttributeValidator(metadata, attribute, modelLocalizer.IfCantThen(globalLocalizer));
                    }
                }

                // Produce a validator for each validation attribute we find
                foreach (var attribute in attributes.OfType<ValidationAttribute>())
                {
                    var globalLocalizer = _localizerProvider.GetLocalizer(attribute.GetType().FullName);
                    yield return GetDataAnnotationsAttributeValidator(metadata, attribute, modelLocalizer.IfCantThen(globalLocalizer));
                }

                // Produce a validator if the type supports IValidatableObject
                if (typeof(IValidatableObject).IsAssignableFrom(metadata.Type))
                {
                    var localizer = _localizerProvider.GetLocalizer(metadata.Type.FullName);
                    yield return GetValidatableObjectValidator(metadata, localizer);
                }
            }
            finally
            {
                _adaptersLock.ExitReadLock();
            }
        }

        private IValidator GetDataAnnotationsAttributeValidator(ObjectMetadata metadata, ValidationAttribute attribute, Localizer localizer)
        {
            // Allow dependency injection in validation attributes
            //_compositionContainerAccessor.Container.SatisfyImportsOnce(attribute);
            // ToDo [10200147]: Provide dependency injection for validation attributes. 
            // It is already disabled because of its weird behavior in some attributes (such as RangeAttribute)

            var attributeType = attribute.GetType();
            var factory = _attributeFactories.Where(f => f.Key == attributeType).Select(f => f.Value).SingleOrDefault() ??
                          _attributeFactories.Where(f => f.Key.IsAssignableFrom(attributeType)).Select(f => f.Value).SingleOrDefault() ??
                          _defaultAttributeFactory;

            var validator = factory(metadata, attribute, localizer);
            // Allow dependency injection in validators
            _compositionContainerAccessor.Container.SatisfyImportsOnce(validator);
            return validator;
        }

        private IValidator GetValidatableObjectValidator(ObjectMetadata metadata, Localizer localizer)
        {
            var factory = _validatableFactories.Where(f => f.Key == metadata.Type).Select(f => f.Value).SingleOrDefault() ??
                          _validatableFactories.Where(f => f.Key.IsAssignableFrom(metadata.Type)).Select(f => f.Value).SingleOrDefault() ??
                          _defaultValidatableFactory;

            var validator = factory(metadata, localizer);
            // Allow dependency injection in validators
            _compositionContainerAccessor.Container.SatisfyImportsOnce(validator);
            return validator;
        }


        #region Validation attribute adapter registration

        /// <summary>
        /// Registers an adapter type for the given <see cref="attributeType"/>, which must be
        /// derieved from <see cref="ValidationAttribute"/>. The adapter type must also be deriven
        /// from <see cref="IValidator"/> and it must contain a public constructor takes two
        /// parameters of type <see cref="ObjectMetadata"/> and <see cref="attributeType"/>.
        /// </summary>
        public static void RegisterAdapter(Type attributeType, Type adapterType)
        {
            ValidateAttributeType(attributeType);
            ValidateAdapterType(adapterType);
            var activator = GetAttributeAdapterActivator(attributeType, adapterType);
            _adaptersLock.EnterWriteLock();
            try
            {
                _attributeFactories[attributeType] = (metadata, attribute, localizer) => (IValidator)activator(metadata, attribute, localizer);
            }
            finally
            {
                _adaptersLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Registers an adapter factory for the given <see cref="attributeType"/>, which must be
        /// derieved from <see cref="ValidationAttribute"/>.
        /// </summary>
        public static void RegisterAdapterFactory(Type attributeType, DataAnnotationsAttributeAdapterFactory factory)
        {
            ValidateAttributeType(attributeType);
            Throw.IfArgumentNull(factory, "factory");
            _adaptersLock.EnterWriteLock();
            try
            {
                _attributeFactories[attributeType] = factory;
            }
            finally
            {
                _adaptersLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Registers the default adapter type for attributes derieved from
        /// <see cref="ValidationAttribute"/>. The adapter type must also be deriven
        /// from <see cref="IValidator"/> and it must contain a public constructor
        /// takes two parameters of type <see cref="ObjectMetadata"/> and <see cref="ValidationAttribute"/>.
        /// </summary>
        public static void RegisterDefaultAdapter(Type adapterType)
        {
            ValidateAdapterType(adapterType);
            var activator = GetAttributeAdapterActivator(typeof(ValidationAttribute), adapterType);
            _defaultAttributeFactory = (metadata, attribute, localizer) => (IValidator)activator(metadata, attribute, localizer);
        }

        /// <summary>
        /// Registers the default adapter factory for attributes derieved from
        /// <see cref="ValidationAttribute"/>.
        /// </summary>
        public static void RegisterDefaultAdapterFactory(DataAnnotationsAttributeAdapterFactory factory)
        {
            Throw.IfArgumentNull(factory, "factory");
            _defaultAttributeFactory = factory;
        }

        #endregion

        #region IValidatableObject adapter registration

        /// <summary>
        /// Registers an adapter type for the given <see cref="objectType"/>, which must
        /// implement <see cref="IValidatableObject"/>. The adapter type must also be deriven
        /// from <see cref="IValidator"/> and it must contain a public constructor takes
        /// a parameter of type <see cref="ObjectMetadata"/>.
        /// </summary>
        public static void RegisterValidatableObjectAdapter(Type objectType, Type adapterType)
        {
            ValidateValidatableObjectType(objectType);
            ValidateAdapterType(adapterType);
            var activator = GetValidatableAdapterActivator(adapterType);
            _adaptersLock.EnterWriteLock();
            try
            {
                _validatableFactories[objectType] = (metadata, localizer) => (IValidator)activator(metadata, localizer);
            }
            finally
            {
                _adaptersLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Registers an adapter factory for the given <see cref="objectType"/>, which must
        /// implement <see cref="IValidatableObject"/>.
        /// </summary>
        public static void RegisterValidatableObjectAdapterFactory(Type objectType, DataAnnotationsValidatableObjectAdapterFactory factory)
        {
            ValidateValidatableObjectType(objectType);
            Throw.IfArgumentNull(factory, "factory");
            _adaptersLock.EnterWriteLock();
            try
            {
                _validatableFactories[objectType] = factory;
            }
            finally
            {
                _adaptersLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Registers the default adapter type for objects implementing
        /// <see cref="IValidatableObject"/>. The adapter type must be deriven
        /// from <see cref="IValidator"/> and it must contain a public constructor
        /// takes a parameter of type <see cref="ObjectMetadata"/>.
        /// </summary>
        public static void RegisterDefaultValidatableObjectAdapter(Type adapterType)
        {
            ValidateAdapterType(adapterType);
            var activator = GetValidatableAdapterActivator(adapterType);
            _defaultValidatableFactory = (metadata, localizer) => (IValidator)activator(metadata, localizer);
        }

        /// <summary>
        /// Registers the default adapter factory for objects implementing
        /// <see cref="IValidatableObject"/>.
        /// </summary>
        public static void RegisterDefaultValidatableObjectAdapterFactory(DataAnnotationsValidatableObjectAdapterFactory factory)
        {
            Throw.IfArgumentNull(factory, "factory");
            _defaultValidatableFactory = factory;
        }

        #endregion

        #region Private Helper Methods

        private static ObjectActivator GetAttributeAdapterActivator(Type attributeType, Type adapterType)
        {
            var attributeAdapterActivator = ObjectFactory.GetActivator(adapterType, typeof(ObjectMetadata), attributeType, typeof(Localizer));
            Throw.IfNull(attributeAdapterActivator)
                .AnArgumentException("The type {0} must have a public constructor which accepts two parameters of types {1} and {2}."
                .FormatWith(adapterType.FullName, typeof(ObjectMetadata).FullName, attributeType.FullName), "adapterType");
            return attributeAdapterActivator;
        }

        private static ObjectActivator GetValidatableAdapterActivator(Type adapterType)
        {
            var validatableAdapterActivator = ObjectFactory.GetActivator(adapterType, typeof(ObjectMetadata), typeof(Localizer));
            Throw.IfNull(validatableAdapterActivator)
                .AnArgumentException("The type {0} must have a public constructor which accepts a parameter of type {1}."
                .FormatWith(adapterType.FullName, typeof(ObjectMetadata).FullName), "adapterType");
            return validatableAdapterActivator;
        }

        private static void ValidateAdapterType(Type adapterType)
        {
            Throw.IfArgumentNull(adapterType, "adapterType");
            Throw.IfNot(typeof(IValidator).IsAssignableFrom(adapterType))
                .AnArgumentException("The type {0} must derive from {1}"
                .FormatWith(adapterType.FullName, typeof(IValidator).FullName), "adapterType");
        }

        private static void ValidateAttributeType(Type attributeType)
        {
            Throw.IfArgumentNull(attributeType, "attributeType");
            Throw.IfNot(typeof(ValidationAttribute).IsAssignableFrom(attributeType))
                .AnArgumentException("The type {0} must derive from {1}"
                .FormatWith(attributeType.FullName, typeof(ValidationAttribute).FullName), "attributeType");
        }

        private static void ValidateValidatableObjectType(Type objectType)
        {
            Throw.IfArgumentNull(objectType, "objectType");
            Throw.IfNot(typeof(IValidatableObject).IsAssignableFrom(objectType))
                .AnArgumentException("The type {0} must derive from {1}"
                .FormatWith(objectType.FullName, typeof(IValidatableObject).FullName), "objectType");

        }
        #endregion
    }

}
