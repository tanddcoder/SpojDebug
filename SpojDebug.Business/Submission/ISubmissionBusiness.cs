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
        Task InstantDownLoadSubmissionAsync(int accountId, string accountName, int submissionId);
    }
}
