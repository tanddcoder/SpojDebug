using SpojDebug.Core.Entities.Submission;
using SpojDebug.Business.Submission;
using SpojDebug.Service.Logic.Base;
using SpojDebug.Service.Submission;

namespace SpojDebug.Service.Logic.Submission
{
    public class SubmissionService : Service<ISubmissionBusiness, SubmissionEntity>, ISubmissionService
    {
        
    }
}
