using SpojDebug.Business.Result;
using SpojDebug.Business.Logic.Base;
using SpojDebug.Core.Entities.Result;
using SpojDebug.Data.Repositories.Result;

namespace SpojDebug.Business.Logic.Result
{
    public class ResultBusiness : Business<IResultRepository, ResultEntity>, IResultBusiness
    {
        protected ResultBusiness(IResultRepository repository) : base(repository)
        {
        }
    }
}
