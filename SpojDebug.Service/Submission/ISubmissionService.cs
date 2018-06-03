using SpojDebug.Service.Base;
using SpojDebug.Core.Entities.Submission;
using SpojDebug.Core.Models.ApplicationResponse;
using SpojDebug.Core.Models.Submission;
using System.Collections.Generic;

namespace SpojDebug.Service.Submission
{
    public interface ISubmissionService : IService<SubmissionEntity>
    {
        ApplicationResult<List<SubmissionHomeModel>> GetUserSubmission(string userId);
        ApplicationResult<SubmissionFirstFailModel> GetFirstFailForFailer(int submissionId);
    }
}
