using SpojDebug.Core.AppSetting;
using SpojDebug.Core.Entities.TestCase;
using SpojDebug.Data.EF.Base;
using SpojDebug.Data.EF.Contexts;
using SpojDebug.Data.Repositories.TestCase;

namespace SpojDebug.Data.EF.Repositories.TestCase
{
    public class TestCaseRepository : Repository<SpojDebugDbContext, TestCaseInfoEntity>, ITestCaseRepository
    {
        public TestCaseRepository(SpojDebugDbContext context, SystemInfo systemInfo) : base(context, systemInfo)
        {
        }
    }
}
