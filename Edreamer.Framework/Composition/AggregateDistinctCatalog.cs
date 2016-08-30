using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;

namespace Edreamer.Framework.Composition
{
    public class AggregateDistinctCatalog: AggregateCatalog
    {
        private static readonly IEqualityComparer<Tuple<ComposablePartDefinition, ExportDefinition>> ExportEqualityComparer =
                new StringRepresentationEqualityComparer<Tuple<ComposablePartDefinition, ExportDefinition>>();
        private static readonly IEqualityComparer<ComposablePartDefinition> PartEqualityComparer =
            new StringRepresentationEqualityComparer<ComposablePartDefinition>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="AggregateDistinctCatalog"/> class.
        /// </summary>
        public AggregateDistinctCatalog()
            : this((IEnumerable<ComposablePartCatalog>)null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AggregateDistinctCatalog"/> class 
        ///     with the specified catalogs.
        /// </summary>
        /// <param name="catalogs">
        ///     An <see cref="Array"/> of <see cref="ComposablePartCatalog"/> objects to add to the 
        ///     <see cref="AggregateCatalog"/>.
        /// </param>
        public AggregateDistinctCatalog(params ComposablePartCatalog[] catalogs)
            : this((IEnumerable<ComposablePartCatalog>)catalogs)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AggregateDistinctCatalog"/> class
        ///     with the specified catalogs.
        /// </summary>
        /// <param name="catalogs">
        ///     An <see cref="IEnumerable{T}"/> of <see cref="ComposablePartCatalog"/> objects to add
        ///     to the <see cref="AggregateCatalog"/>; or <see langword="null"/> to 
        ///     create an <see cref="AggregateCatalog"/> that is empty.
        /// </param>
        public AggregateDistinctCatalog(IEnumerable<ComposablePartCatalog> catalogs)
            : base(catalogs)
        {
        }

        public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
        {
            return base.GetExports(definition).Distinct(ExportEqualityComparer);
        }

        public override IQueryable<ComposablePartDefinition> Parts
        {
            get
            {
                return base.Parts.Distinct(PartEqualityComparer);
            }
        }

        public override IEnumerator<ComposablePartDefinition> GetEnumerator()
        {
            return Catalogs.SelectMany(catalog => catalog).Distinct(PartEqualityComparer).GetEnumerator();
        }

        private class StringRepresentationEqualityComparer<T> : IEqualityComparer<T>
        {
            public bool Equals(T x, T y)
            {
                return (x.ToString() == y.ToString());
            }

            public int GetHashCode(T obj)
            {
                return obj.ToString().GetHashCode();
            }
        }
    }
}
