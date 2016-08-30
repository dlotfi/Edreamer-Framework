using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Reflection;
using Edreamer.Framework.Data.Infrastructure;
using Edreamer.Framework.DataAnnotations;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Localization;
using Edreamer.Framework.Validation;
//using StringPropertyConfiguration = System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive.StringPropertyConfiguration;
using ValidationException = Edreamer.Framework.Validation.ValidationException;
using ValidationResult = Edreamer.Framework.Validation.ValidationResult;

namespace Edreamer.Framework.Data.EntityFramework
{
    /// <summary>
    /// This data context class is based on <see cref="System.Data.Entity.DbContext" />.
    /// </summary>
    /// <remarks>
    /// 1. By default lazy loading, proxy creation, automatic change detection is deactive and validation
    /// on save is active.
    /// 2. If you want to use universal validation service, provide it through ValidationService property.
    /// Otherwise Entity Framework's default validation mechanism is used. 
    /// 4. If you want to localize validation errors, provide a localizer provider through LocalizerProvider
    /// property.
    /// </remarks>
    public class DataContext : DbContext, IDataContext
    {
        protected IDictionary<Type, object> Repositories { get; private set; }

        public IValidationService ValidationService { get; set; }
        public ILocalizerProvider LocalizerProvider { get; set; }
        public event EventHandler SavedChanges;

        #region Constructors
        /// <summary>
        /// Constructs a new context instance using conventions to create the name of
        /// the database to which a connection will be made. The by-convention name is
        /// the full name (namespace + class name) of the derived context class.  See
        /// the <see cref="DbContext"/> class remarks for how this is used to create a connection.
        /// </summary>
        protected DataContext()
        {
            Initialize();
        }

        /// <summary>
        /// Constructs a new context instance using conventions to create the name of
        /// the database to which a connection will be made. The by-convention name is
        /// the full name (namespace + class name) of the derived context class.  See
        /// the <see cref="DbContext"/> class remarks for how this is used to create a connection.
        /// </summary>
        /// <param name="model">The model that will back this context.</param>
        protected DataContext(DbCompiledModel model)
            : base(model)
        {
            Initialize();
        }

        /// <summary>
        /// Constructs a new context instance using the given string as the name or connection
        /// string for the database to which a connection will be made.  See the <see cref="DbContext"/> 
        /// class remarks for how this is used to create a connection.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        public DataContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            Initialize();
        }

        /// <summary>
        /// Constructs a new context instance using the existing connection to connect
        /// to a database.  The connection will not be disposed when the context is disposed.
        /// </summary>
        /// <param name="existingConnection">An existing connection to use for the new context.</param>
        /// <param name="contextOwnsConnection">
        /// If set to true the connection is disposed when the context is disposed, otherwise
        /// the caller must dispose the connection.
        /// </param>
        public DataContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
            Initialize();
        }


        /// <summary>
        /// Constructs a new context instance around an existing ObjectContext.  An existing
        /// ObjectContext to wrap with the new context.  If set to true the ObjectContext
        /// is disposed when the DbContext is disposed, otherwise the caller must dispose
        /// the connection.
        /// </summary>
        /// <param name="objectContext">An existing ObjectContext to wrap with the new context.</param>
        /// <param name="dbContextOwnsObjectContext">
        /// If set to true the ObjectContext is disposed when the DbContext is disposed,
        /// otherwise the caller must dispose the connection.
        /// </param>
        public DataContext(ObjectContext objectContext, bool dbContextOwnsObjectContext)
            : base(objectContext, dbContextOwnsObjectContext)
        {
            Initialize();
        }


        /// <summary>
        /// Constructs a new context instance using the given string as the name or connection
        /// string for the database to which a connection will be made, and initializes
        /// it from the given model.  See the class remarks for how this is used to create
        /// a connection.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        /// <param name="model">The model that will back this context.</param>
        public DataContext(string nameOrConnectionString, DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {
            Initialize();
        }

        /// <summary>
        /// Constructs a new context instance using the existing connection to connect
        /// to a database, and initializes it from the given model.  The connection will
        /// not be disposed when the context is disposed.  An existing connection to
        /// use for the new context.  The model that will back this context.  If set
        /// to true the connection is disposed when the context is disposed, otherwise
        /// the caller must dispose the connection.
        /// </summary>
        /// <param name="existingConnection">An existing connection to use for the new context.</param>
        /// <param name="model">The model that will back this context.</param>
        /// <param name="contextOwnsConnection">
        /// If set to true the connection is disposed when the context is disposed, otherwise
        /// the caller must dispose the connection.
        /// </param>
        public DataContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
            Initialize();
        }

        private void Initialize()
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.AutoDetectChangesEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Configuration.ValidateOnSaveEnabled = true;

            Repositories = new Dictionary<Type, object>();
        }
        #endregion

        protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry, IDictionary<object, object> items)
        {
            var result = ValidationService == null
                             ? base.ValidateEntity(entityEntry, items)
                             : ValidateEntity(ValidationService, entityEntry);
            Throw.IfNull(entityEntry.Entity).A<InvalidOperationException>("A null entity cannot be validated.");

            var objectContext = (this as IObjectContextAdapter).ObjectContext;
            var updatedProperties = objectContext.ObjectStateManager.GetObjectStateEntry(entityEntry.Entity).GetModifiedProperties();
            result.ValidationErrors
                    .Where(ve => !updatedProperties.Contains(ve.PropertyName, StringComparer.OrdinalIgnoreCase)).ToList()
                    .ForEach(ve => result.ValidationErrors.Remove(ve));

            return result;
        }

        public override int SaveChanges()
        {
            int affectedObjects = 0;
            try
            {
                ChangeTracker.DetectChanges();
                affectedObjects = base.SaveChanges();
                if (SavedChanges != null)
                {
                    SavedChanges(this, EventArgs.Empty);
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Throw.Now.A<DataConcurrencyException>(
                    "A concurrency error occured while trying to save changes of the current data context.", ex);
            }
            catch (DbEntityValidationException ex)
            {
                if (ValidationService == null)
                {
                    Throw.Now.A<ValidationException>(
                        "A validation error occured while trying to save changes of the current data context.", ex);
                }
                Throw.Now.A<ValidationException>(
                        "A validation error occured while trying to save changes of the current data context.", ex,
                        ConvertToValidationResult(ex.EntityValidationErrors));
            }
            return affectedObjects;
        }

        public virtual IRepository<T> Repository<T>() where T : class
        {
            if (!Repositories.ContainsKey(typeof(T)))
            {
                Repositories.Add(typeof(T), new Repository<T>(this));
            }
            return Repositories[typeof(T)] as IRepository<T>;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Conventions.Add<CompositeDataAnnotationsAttributeConvention>();
        }

        private static DbEntityValidationResult ValidateEntity(IValidationService validationService, DbEntityEntry entityEntry)
        {
            var validationResult = new DbEntityValidationResult(entityEntry, new List<DbValidationError>());
            if (entityEntry.State == System.Data.Entity.EntityState.Added || entityEntry.State == System.Data.Entity.EntityState.Modified)
            {
                foreach (var result in validationService.Validate(entityEntry.Entity))
                {
                    validationResult.ValidationErrors.Add(new ValidationError(result.MemberName, result.LocalizedMessage, result));
                }
            }
            return validationResult;
        }

        private IEnumerable<ValidationResult> ConvertToValidationResult(IEnumerable<DbEntityValidationResult> entityValidationResults)
        {
            foreach (var entityValidationResult in entityValidationResults)
            {
                foreach (var validationError in entityValidationResult.ValidationErrors)
                {
                    var error = validationError as ValidationError;
                    var localizer = LocalizerProvider == null
                                        ? NullLocalizer.Instance
                                        : LocalizerProvider.GetLocalizer(entityValidationResult.Entry.Entity.GetType().FullName)
                                          .IfCantThen(LocalizerProvider.GetLocalizer(GetType().FullName));
                    yield return error != null
                                     ? error.ValidationResult
                                     : new ValidationResult(
                                           entityValidationResult.Entry.Entity,
                                           validationError.PropertyName,
                                           validationError.ErrorMessage); // ToDo [10200216]: Provide a mechanism to localize Entity Framework built-in validation errors
                }
            }
        }

        private class ValidationError : DbValidationError
        {
            public ValidationError(string propertyName, string errorMessage, ValidationResult validationResult)
                : base(propertyName, errorMessage)
            {
                Throw.IfArgumentNull(validationResult, "validationResult");
                ValidationResult = validationResult;
            }

            public ValidationResult ValidationResult { get; private set; }
        }

        //private class CompositeDataAnnotationsAttributeConvention : IConfigurationConvention<PropertyInfo, PrimitivePropertyConfiguration>
        //{
        //    public void Apply(PropertyInfo memberInfo, Func<PrimitivePropertyConfiguration> configuration)
        //    {
        //        var attributes = memberInfo.GetCustomAttributes(typeof(CompositeDataAnnotationsAttribute), false).OfType<CompositeDataAnnotationsAttribute>();
        //        foreach (var attribute in attributes.SelectMany(a => a.GetAttributes()))
        //        {
        //            if (attribute is StringLengthAttribute)
        //            {
        //                var stringPropertyConfiguration = configuration() as StringPropertyConfiguration;
        //                Throw.IfNull(stringPropertyConfiguration)
        //                    .A<InvalidOperationException>("StringLengthAttribute cannot be applied to non string properties.");
        //                var stringLengthAttribute = attribute as StringLengthAttribute;
        //                if (stringLengthAttribute.MaximumLength == int.MaxValue || stringLengthAttribute.MaximumLength == -1)
        //                {
        //                    stringPropertyConfiguration.IsMaxLength = true;
        //                }
        //                else if (stringLengthAttribute.MaximumLength == stringLengthAttribute.MinimumLength && stringLengthAttribute.MinimumLength > 0)
        //                {
        //                    stringPropertyConfiguration.IsMaxLength = true;
        //                    stringPropertyConfiguration.IsFixedLength = true;
        //                    stringPropertyConfiguration.MaxLength = stringLengthAttribute.MaximumLength;
        //                }
        //                else
        //                {
        //                    stringPropertyConfiguration.MaxLength = stringLengthAttribute.MaximumLength;
        //                }
        //            }
        //            else if (attribute is RequiredAttribute)
        //            {
        //                configuration().IsNullable = false;
        //            }
        //            // ToDo-Low [01131925]: Define conventions for other attributes like MaxLength, MinLength, Key, Timestamp, Column, ...
        //        }
        //    }
        //}
    }
}
