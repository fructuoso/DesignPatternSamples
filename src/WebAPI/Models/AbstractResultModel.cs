using System.Collections.Generic;

namespace DesignPatternSamples.WebAPI.Models
{
    public abstract class AbstractResultModel<TEntity> : IResultModel<TEntity>
    {
        public TEntity Data { get; protected set; }

        public abstract bool HasSucceeded { get; }

        public IEnumerable<ResultDetail> Details { get; protected set; }
    }
}