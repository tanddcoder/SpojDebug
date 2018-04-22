using SpojDebug.Business.Submission;
using SpojDebug.Business.Logic.Base;
using SpojDebug.Core.Entities.Submission;
using SpojDebug.Data.Repositories.Submission;
using AutoMapper;

namespace SpojDebug.Business.Logic.Submission
{
    public class SubmissionBusiness : Business<ISubmissionRepository, SubmissionEntity>, ISubmissionBusiness
    {
        protected SubmissionBusiness(ISubmissionRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
