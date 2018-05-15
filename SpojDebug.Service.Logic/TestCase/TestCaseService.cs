using SpojDebug.Core.Entities.TestCase;
using SpojDebug.Service.Logic.Base;
using SpojDebug.Business.TestCase;
using SpojDebug.Service.TestCase;

namespace SpojDebug.Service.Logic.TestCase
{
    public class TestCaseService : Service<ITestCaseBusiness, TestCaseInfoEntity>, ITestCaseService
    {
        
    }
}
