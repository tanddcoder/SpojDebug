using SpojDebug.Core.AppSetting;
using SpojDebug.Core.Entities.Submission;
using SpojDebug.Data.EF.Base;
using SpojDebug.Data.EF.Contexts;
using SpojDebug.Data.Repositories.Submission;

namespace SpojDebug.Data.EF.Repositories.Submission
{
    public class SubmissionRepository : Repository<SpojDebugDbContext, SubmissionEntity>, ISubmissionRepository
    {
        public SubmissionRepository(SpojDebugDbContext context, SystemInfo systemInfo) : base(context, systemInfo)
        {
        }
    }
}
