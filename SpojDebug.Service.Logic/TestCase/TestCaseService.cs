using SpojDebug.Core.Entities.TestCase;
using SpojDebug.Service.Logic.Base;
using SpojDebug.Business.TestCase;
using SpojDebug.Service.TestCase;
using SpojDebug.Core.Models.ApplicationResponse;
using SpojDebug.Core.Models.TestCase;
using System.Threading.Tasks;

namespace SpojDebug.Service.Logic.TestCase
{
    public class TestCaseService : Service<ITestCaseBusiness, TestCaseInfoEntity>, ITestCaseService
    {
        private readonly ITestCaseBusiness _testCaseBusiness;

        public TestCaseService(ITestCaseBusiness testCaseBusiness)
        {
            _testCaseBusiness = testCaseBusiness;
        }

        public async Task<ApplicationResult<TestCaseResponseModel>> GetFirstFailForFailer(int submissionId, string userId)
        {
            var response = await _testCaseBusiness.GetFirstFailForFailerAsync(submissionId, userId);

            return ApplicationResult<TestCaseResponseModel>.Ok(response);
        }

        public async Task<ApplicationResult<TestCaseDetailResonseModel>> GetTestCaseDetail(int testCaseSeq)
        {
            var response = await _testCaseBusiness.GetTestCaseDetailAsync(testCaseSeq);

            return ApplicationResult<TestCaseDetailResonseModel>.Ok(response);
        }
    }
}
