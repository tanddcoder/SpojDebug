using SpojDebug.Business.Base;
using SpojDebug.Core.Entities.TestCase;
using SpojDebug.Core.Models.TestCase;

namespace SpojDebug.Business.TestCase
{
    public interface ITestCaseBusiness : IBusiness<TestCaseInfoEntity>
    {
        TestCaseDetailResonseModel GetTestCaseDetail(int testCaseSeq);
        TestCaseResponseModel GetFirstFailForFailer(int submissionId, string userId);
    }
}
