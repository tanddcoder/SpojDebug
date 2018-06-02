using SpojDebug.Business.Result;
using SpojDebug.Business.Logic.Base;
using SpojDebug.Core.Entities.Result;
using SpojDebug.Data.Repositories.Result;
using AutoMapper;

namespace SpojDebug.Business.Logic.Result
{
    public class ResultBusiness : Business<IResultRepository, ResultEntity>, IResultBusiness
    {
        public ResultBusiness(IResultRepository repository,IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
