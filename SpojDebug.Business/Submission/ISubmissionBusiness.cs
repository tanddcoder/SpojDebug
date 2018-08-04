using SpojDebug.Business.Base;
using SpojDebug.Core.Entities.Submission;
using SpojDebug.Core.Models.Submission;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpojDebug.Business.Submission
{
    public interface ISubmissionBusiness : IBusiness<SubmissionEntity>
    {
        Task<List<SubmissionHomeModel>> GetUserSubmissionAsync(string userId);
        void InstantDownLoadSubmission(int accountId, string accountName, int submissionId);
    }
}
