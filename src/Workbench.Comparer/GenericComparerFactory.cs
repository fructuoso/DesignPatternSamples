using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Workbench.Comparer
{
    public class GenericComparerFactory<TEntity> : IEqualityComparer<TEntity>
    {
        private Func<TEntity, object> Predicate { get; set; }

        private GenericComparerFactory() { }

        public static GenericComparerFactory<TEntity> Create(Func<TEntity, object> predicate)
        {
            return new GenericComparerFactory<TEntity>() { Predicate = predicate };
        }

        public bool Equals([AllowNull] TEntity x, [AllowNull] TEntity y)
        {
            return Predicate(x).Equals(Predicate(y));
        }

        public int GetHashCode([DisallowNull] TEntity obj)
        {
            return Predicate(obj).GetHashCode();
        }
    }
}
