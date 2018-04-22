using SpojDebug.Business.Problem;
using SpojDebug.Business.Logic.Base;
using SpojDebug.Core.Entities.Problem;
using SpojDebug.Data.Repositories.Problem;
using AutoMapper;

namespace SpojDebug.Business.Logic.Problem
{
    public class ProblemBusiness : Business<IProblemRepository, ProblemEntity>, IProblemBusiness
    {
        protected ProblemBusiness(IProblemRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
