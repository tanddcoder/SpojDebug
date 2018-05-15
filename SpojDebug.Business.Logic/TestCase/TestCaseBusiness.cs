using SpojDebug.Business.TestCase;
using SpojDebug.Business.Logic.Base;
using SpojDebug.Core.Entities.TestCase;
using SpojDebug.Data.Repositories.TestCase;
using AutoMapper;

namespace SpojDebug.Business.Logic.TestCase
{
    public class TestCaseBusiness : Business<ITestCaseRepository, TestCaseInfoEntity>, ITestCaseBusiness
    {
        protected TestCaseBusiness(ITestCaseRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
