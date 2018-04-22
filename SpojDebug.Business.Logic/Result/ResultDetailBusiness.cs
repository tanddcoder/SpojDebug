using SpojDebug.Business.Result;
using SpojDebug.Business.Logic.Base;
using SpojDebug.Core.Entities.ResultDetail;
using SpojDebug.Data.Repositories.Result;
using AutoMapper;

namespace SpojDebug.Business.Logic.Result
{
    public class ResultDetailBusiness : Business<IResultDetailRepository, ResultDetailEntity>, IResultDetailBusiness
    {
        protected ResultDetailBusiness(IResultDetailRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
