using System.Collections.Generic;

namespace DesignPatternSamples.WebAPI.Models
{
    public class FailureResultModel<TEntity> : AbstractResultModel<TEntity>
    {
        public override bool HasSucceeded => false;

        public FailureResultModel(TEntity data, IEnumerable<ResultDetail> details)
        {
            Data = data;
            Details = details;
        }
    }

    public class FailureResultModel : SuccessResultModel<object>
    {
        public FailureResultModel(IEnumerable<ResultDetail> details) : base(null, details) { }
    }
}