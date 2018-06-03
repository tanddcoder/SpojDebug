using SpojDebug.Service.Base;
using SpojDebug.Core.Entities.TestCase;
using SpojDebug.Core.Models.ApplicationResponse;
using SpojDebug.Core.Models.TestCase;

namespace SpojDebug.Service.TestCase
{
    public interface ITestCaseService : IService<TestCaseInfoEntity>
    {
        ApplicationResult<TestCaseDetailResonseModel> GetTestCaseDetail(int testCaseSeq);
        ApplicationResult<TestCaseResponseModel> GetFirstFailForFailer(int submissionId, string userId);
    }
}
