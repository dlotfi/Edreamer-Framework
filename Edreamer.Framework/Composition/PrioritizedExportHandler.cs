using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Edreamer.Framework.Helpers;
using Edreamer.Framework.Module;
using MefContrib.Hosting.Interception;

namespace Edreamer.Framework.Composition
{
    internal class PrioritizedExportHandler : IExportHandler
    {
        private ComposablePartCatalog _interceptedCatalog;
        private readonly Lazy<IModuleManager> _moduleManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrioritizedExportHandler"/> class.
        /// </summary>
        /// <param name="moduleManagerFactory">The module manager factory.</param>
        public PrioritizedExportHandler(Func<IModuleManager> moduleManagerFactory)
        {
            Throw.IfArgumentNull(moduleManagerFactory, "moduleManagerFactory");
            _moduleManager = new Lazy<IModuleManager>(moduleManagerFactory);
        }

        public void Initialize(ComposablePartCatalog interceptedCatalog)
        {
            Throw.IfArgumentNull(interceptedCatalog, "interceptedCatalog");
            _interceptedCatalog = interceptedCatalog;
        }

        public IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition, IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> exports)
        {
            Throw.IfArgumentNull(definition, "definition");

            // RiskPoint: I have modified the ImportDefinition by scanning the constraint expression and removes Priority and Module metadata requirements.
            // Remove contsraints about Priority and Module metadata
            bool needModuleMetadata, needPriorityMetadata;
            var refinedDefinition = GetRefinedDefinition(definition, out needModuleMetadata, out needPriorityMetadata);
            exports = (needModuleMetadata || needPriorityMetadata)
                          ? _interceptedCatalog.GetExports(refinedDefinition).ToList()
                          : exports.ToList();
            
            if (!exports.Any())
                yield break;

            exports = definition.Cardinality != ImportCardinality.ZeroOrMore
                ? exports.GetTopPriorityItems(e => GetPriority(e.Item1)).ToList() // Find top priority items
                : exports.OrderByDescending(e => GetPriority(e.Item1)).ToList(); // Sort exports according to their priorities

            // Add Priority and Module metadata to export metadata
            foreach (var export in exports)
            {
                var composablePartDefinition = export.Item1;
                var exportDefinition = export.Item2;
                var metadataChanged = false;
                IDictionary<string, object> metadataDictionary = new Dictionary<string, object>(exportDefinition.Metadata);
                if (needPriorityMetadata && !metadataDictionary.ContainsKey(PriorityMetadataName))
                {
                    metadataDictionary.Add(PriorityMetadataName, GetPriority(composablePartDefinition));
                    metadataChanged = true;
                }
                if (needModuleMetadata && !metadataDictionary.ContainsKey(ModuleMetadataName))
                {
                    metadataDictionary.Add(ModuleMetadataName, GetModuleName(composablePartDefinition, _moduleManager.Value));
                    metadataChanged = true;
                }
                // RiskPoint: I use reflection to change a private readonly member "_metadata" in ExportDefinition
                if (metadataChanged)
                {
                    // It's important to use 'exportDefinition.GetType()' instead of 'typeof(ExportDefinition)'
                    exportDefinition.GetType().GetFieldInfo("_metadata").SetValue(exportDefinition, metadataDictionary);
                }
                yield return new Tuple<ComposablePartDefinition, ExportDefinition>(composablePartDefinition, exportDefinition);
            }
        }

        #region Private Methods

        private const string PriorityMetadataName = "Priority";
        private const string ModuleMetadataName = "ModuleName";

        private static readonly Expression PriorityMetadataContainsKeyExpression;
        private static readonly Expression PriorityMetadataOfTypeExpression;
        private static readonly Expression ModuleMetadataContainsKeyExpression;
        private static readonly Expression ModuleMetadataOfTypeExpression;

        // Based on MEF 2 Preview 5 source code (SontraintServices.cs)
        private static readonly PropertyInfo ExportDefinitionMetadataProperty = typeof(ExportDefinition).GetProperty("Metadata");
        private static readonly MethodInfo MetadataContainsKeyMethod = typeof(IDictionary<string, object>).GetMethod("ContainsKey");
        private static readonly MethodInfo MetadataItemMethod = typeof(IDictionary<string, object>).GetMethod("get_Item");
        private static readonly MethodInfo TypeIsInstanceOfTypeMethod = typeof(Type).GetMethod("IsInstanceOfType");

        static PrioritizedExportHandler()
        {
            // Based on MEF 2 Preview 5 source code (SontraintServices.cs)
            ParameterExpression parameter = Expression.Parameter(typeof(ExportDefinition), "exportDefinition");

            PriorityMetadataContainsKeyExpression = CreateMetadataContainsKeyExpression(parameter, PriorityMetadataName);
            PriorityMetadataOfTypeExpression = CreateMetadataOfTypeExpression(parameter, PriorityMetadataName, typeof(int));
            ModuleMetadataContainsKeyExpression = CreateMetadataContainsKeyExpression(parameter, ModuleMetadataName);
            ModuleMetadataOfTypeExpression = CreateMetadataOfTypeExpression(parameter, ModuleMetadataName, typeof(string));
        }

        // Based on MEF 2 Preview 5 source code (SontraintServices.cs)
        private static Expression CreateMetadataContainsKeyExpression(ParameterExpression parameter, string constantKey)
        {
            Throw.IfArgumentNull(parameter, "parameter");
            Throw.IfArgumentNull(constantKey, "constantKey");

            // definition.Metadata.ContainsKey(constantKey)
            return Expression.Call(
                        Expression.Property(parameter, ExportDefinitionMetadataProperty),
                        MetadataContainsKeyMethod,
                        Expression.Constant(constantKey));
        }

        // Based on MEF 2 Preview 5 source code (SontraintServices.cs)
        private static Expression CreateMetadataOfTypeExpression(ParameterExpression parameter, string constantKey, Type constantType)
        {
            Throw.IfArgumentNull(parameter, "parameter");
            Throw.IfArgumentNull(constantKey, "constantKey");
            Throw.IfArgumentNull(constantType, "constantType");

            // constantType.IsInstanceOfType(definition.Metadata[constantKey])
            return Expression.Call(
                            Expression.Constant(constantType, typeof(Type)),
                            TypeIsInstanceOfTypeMethod,
                            Expression.Call(
                                Expression.Property(parameter, ExportDefinitionMetadataProperty),
                                MetadataItemMethod,
                                Expression.Constant(constantKey))
                            );
        }

        private static ImportDefinition GetRefinedDefinition(ImportDefinition definition, out bool needModuleMetadata, out bool needPriorityMetadata)
        {
            var constraint = Expression.Lambda<Func<ExportDefinition, bool>>(
                    GetRefinedCondition(definition.Constraint.Body, out needModuleMetadata, out needPriorityMetadata), definition.Constraint.Parameters);
            return new ImportDefinition(constraint, definition.ContractName, definition.Cardinality, definition.IsRecomposable, definition.IsPrerequisite, definition.Metadata);
        }

        private static Expression GetRefinedCondition(Expression condition, out bool needModuleMetadata, out bool needPriorityMetadata)
        {
            Throw.IfArgumentNull(condition, "condition");
            needModuleMetadata = false;
            needPriorityMetadata = false;

            var binaryExpression = condition as BinaryExpression;
            if (binaryExpression != null)
            {
                bool leftNeedModuleMetadata, rightNeedModuleMetadata;
                bool leftNeedPriorityMetadata, rightNeedPriorityMetadata;
                var result = Expression.MakeBinary(binaryExpression.NodeType,
                             GetRefinedCondition(binaryExpression.Left, out leftNeedModuleMetadata, out leftNeedPriorityMetadata),
                             GetRefinedCondition(binaryExpression.Right, out rightNeedModuleMetadata, out rightNeedPriorityMetadata),
                             binaryExpression.IsLiftedToNull,
                             binaryExpression.Method,
                             binaryExpression.Conversion);
                needModuleMetadata = leftNeedModuleMetadata || rightNeedModuleMetadata;
                needPriorityMetadata = leftNeedPriorityMetadata || rightNeedPriorityMetadata;
                return result;
            }

            if (condition.ExpressionEquals(PriorityMetadataContainsKeyExpression) ||
                condition.ExpressionEquals(PriorityMetadataOfTypeExpression))
            {
                needPriorityMetadata = true;
                return Expression.Constant(true);
            }
            if (condition.ExpressionEquals(ModuleMetadataContainsKeyExpression) ||
                condition.ExpressionEquals(ModuleMetadataOfTypeExpression))
            {
                needModuleMetadata = true;
                return Expression.Constant(true);
            }
            return condition;
        }

        private static int GetPriority(ComposablePartDefinition partDefinition)
        {
            Throw.IfArgumentNull(partDefinition, "partDefinition");

            return partDefinition.ContainsPartMetadataWithKey(CompositionConstants.PriorityMetadataName)
                       ? (int)partDefinition.Metadata[CompositionConstants.PriorityMetadataName]
                       : PartPriorityAttribute.Default;
        }

        private static string GetModuleName(ComposablePartDefinition partDefinition, IModuleManager moduleManager)
        {
            Throw.IfArgumentNull(partDefinition, "partDefinition");
            var type = partDefinition.ContainsPartMetadataWithKey(CompositionConstants.TypeMetadataName)
                           ? (Type)partDefinition.Metadata[CompositionConstants.TypeMetadataName]
                           : null;
            return type == null || moduleManager == null
                       ? null
                       : moduleManager.GetModule(type).Name;
        }

        #endregion
    }
}
