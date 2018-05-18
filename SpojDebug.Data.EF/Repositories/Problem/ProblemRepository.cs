using SpojDebug.Core.AppSetting;
using SpojDebug.Core.Entities.Problem;
using SpojDebug.Data.EF.Base;
using SpojDebug.Data.EF.Contexts;
using SpojDebug.Data.Repositories.Problem;

namespace SpojDebug.Data.EF.Repositories.Problem
{
    public class ProblemRepository : Repository<SpojDebugDbContext, ProblemEntity>, IProblemRepository
    {
        public ProblemRepository(SpojDebugDbContext context, SystemInfo systemInfo) : base(context, systemInfo)
        {
        }
    }
}
