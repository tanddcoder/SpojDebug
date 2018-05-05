using SpojDebug.Core.Entities.ProblemDetail;
using SpojDebug.Data.EF.Base;
using SpojDebug.Data.EF.Contexts;

namespace SpojDebug.Data.EF.Repositories.Problem
{
    public class ProblemDetailRepository : Repository<SpojDebugDbContext, ProblemDetailEntity>
    {
        public ProblemDetailRepository(SpojDebugDbContext context) : base(context)
        {
        }
    }
}
