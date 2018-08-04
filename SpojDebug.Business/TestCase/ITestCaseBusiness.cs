using SpojDebug.Business.Base;
using SpojDebug.Core.Entities.TestCase;
using SpojDebug.Core.Models.TestCase;
using System.Threading.Tasks;

namespace SpojDebug.Business.TestCase
{
    public interface ITestCaseBusiness : IBusiness<TestCaseInfoEntity>
    {
        Task<TestCaseDetailResonseModel> GetTestCaseDetailAsync(int testCaseSeq);
        Task<TestCaseResponseModel> GetFirstFailForFailerAsync(int submissionId, string userId);
        Task<TestCaseResponseModel> SearchFirstFailForFailerAsync(int submissionId, string userId);
    }
}
