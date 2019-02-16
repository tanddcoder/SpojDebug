using SpojDebug.Service.Base;
using SpojDebug.Core.Entities.TestCase;
using SpojDebug.Core.Models.ApplicationResponse;
using SpojDebug.Core.Models.TestCase;
using System.Threading.Tasks;

namespace SpojDebug.Service.TestCase
{
    public interface ITestCaseService : IService<TestCaseInfoEntity>
    {
        Task<ApplicationResult<TestCaseDetailResonseModel>> GetTestCaseDetail(int testCaseSeq);
        Task<ApplicationResult<TestCaseResponseModel>> GetFirstFailForFailer(int submissionId, string userId);
        void SyncTestCase(string problemCode);
    }
}
