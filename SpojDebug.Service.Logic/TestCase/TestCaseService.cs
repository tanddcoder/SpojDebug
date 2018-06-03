using SpojDebug.Core.Entities.TestCase;
using SpojDebug.Service.Logic.Base;
using SpojDebug.Business.TestCase;
using SpojDebug.Service.TestCase;
using SpojDebug.Core.Models.ApplicationResponse;
using SpojDebug.Core.Models.TestCase;

namespace SpojDebug.Service.Logic.TestCase
{
    public class TestCaseService : Service<ITestCaseBusiness, TestCaseInfoEntity>, ITestCaseService
    {
        private readonly ITestCaseBusiness _testCaseBusiness;

        public TestCaseService(ITestCaseBusiness testCaseBusiness)
        {
            _testCaseBusiness = testCaseBusiness;
        }

        public ApplicationResult<TestCaseResponseModel> GetFirstFailForFailer(int submissionId, string userId)
        {
            var response = _testCaseBusiness.GetFirstFailForFailer(submissionId, userId);

            return ApplicationResult<TestCaseResponseModel>.Ok(response);
        }

        public ApplicationResult<TestCaseDetailResonseModel> GetTestCaseDetail(int testCaseSeq)
        {
            var response = _testCaseBusiness.GetTestCaseDetail(testCaseSeq);

            return ApplicationResult<TestCaseDetailResonseModel>.Ok(response);
        }
    }
}
