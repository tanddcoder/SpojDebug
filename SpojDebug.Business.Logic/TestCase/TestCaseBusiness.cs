using SpojDebug.Business.TestCase;
using SpojDebug.Business.Logic.Base;
using SpojDebug.Core.Entities.TestCase;
using SpojDebug.Data.Repositories.TestCase;

namespace SpojDebug.Business.Logic.TestCase
{
    public class TestCaseBusiness : Business<ITestCaseRepository, TestCaseEntity>, ITestCaseBusiness
    {
        protected TestCaseBusiness(ITestCaseRepository repository) : base(repository)
        {
        }
    }
}
