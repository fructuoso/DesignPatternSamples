using System.Collections.Generic;

namespace DesignPatternSamples.WebAPI.Models
{
    public class SuccessResultModel<TEntity> : AbstractResultModel<TEntity>
    {
        public override bool HasSucceeded => true;

        public SuccessResultModel(TEntity data)
        {
            Data = data;
        }
        public SuccessResultModel(TEntity data, IEnumerable<ResultDetail> details)
        {
            Data = data;
            Details = details;
        }
    }

    public class SuccessResultModel : SuccessResultModel<object>
    {
        public SuccessResultModel() : base(null, null) { }
    }
}