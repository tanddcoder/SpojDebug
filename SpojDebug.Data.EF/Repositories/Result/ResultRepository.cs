using SpojDebug.Core.Entities.Result;
using SpojDebug.Data.EF.Base;
using SpojDebug.Data.EF.Contexts;
using SpojDebug.Data.Repositories.Result;

namespace SpojDebug.Data.EF.Repositories.Result
{
    public class ResultRepository : Repository<SpojDebugDbContext, ResultEntity> ,IResultRepository
    {
        protected ResultRepository(SpojDebugDbContext dbContext) : base(dbContext)
        {

        }
    }
}
