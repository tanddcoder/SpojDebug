using SpojDebug.Business.Submission;
using SpojDebug.Business.Logic.Base;
using SpojDebug.Core.Entities.Submission;
using SpojDebug.Data.Repositories.Submission;

namespace SpojDebug.Business.Logic.Submission
{
    public class SubmissionBusiness : Business<ISubmissionRepository, SubmissionEntity>, ISubmissionBusiness
    {
        protected SubmissionBusiness(ISubmissionRepository repository) : base(repository)
        {
        }
    }
}
