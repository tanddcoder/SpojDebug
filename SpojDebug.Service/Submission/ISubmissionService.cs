using SpojDebug.Service.Base;
using SpojDebug.Core.Entities.Submission;
using SpojDebug.Core.Models.ApplicationResponse;
using SpojDebug.Core.Models.Submission;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpojDebug.Core.Models.TestCase;

namespace SpojDebug.Service.Submission
{
    public interface ISubmissionService : IService<SubmissionEntity>
    {
        Task<ApplicationResult<List<SubmissionHomeModel>>> GetUserSubmissionAsync(string userId);
        Task<ApplicationResult<SubmissionFirstFailModel>> GetFirstFailForFailerAsync(int submissionId);
        Task<ApplicationResult<TestCaseResponseModel>> SearchSubmssionAsync(string userId, int submissionId);
        Task<ApplicationResult> EnqueueToDownloadAsync(string userId, int value);
    }
}
