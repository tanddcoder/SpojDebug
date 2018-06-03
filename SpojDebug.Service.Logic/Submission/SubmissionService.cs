using SpojDebug.Core.Entities.Submission;
using SpojDebug.Business.Submission;
using SpojDebug.Service.Logic.Base;
using SpojDebug.Service.Submission;
using SpojDebug.Core.Models.ApplicationResponse;
using SpojDebug.Core.Models.Submission;
using System.Collections.Generic;

namespace SpojDebug.Service.Logic.Submission
{
    public class SubmissionService : Service<ISubmissionBusiness, SubmissionEntity>, ISubmissionService
    {
        private readonly ISubmissionBusiness _submissionBusiness;

        public SubmissionService(ISubmissionBusiness submissionBusiness)
        {
            _submissionBusiness = submissionBusiness;
        }

        public ApplicationResult<List<SubmissionHomeModel>> GetUserSubmission(string userId)
        {
            var response = _submissionBusiness.GetUserSubmission(userId);

            return ApplicationResult<List<SubmissionHomeModel>>.Ok(response);
        }
    }
}
