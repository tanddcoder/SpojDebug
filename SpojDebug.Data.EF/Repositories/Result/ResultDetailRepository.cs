using SpojDebug.Core.Entities.ResultDetail;
using SpojDebug.Data.EF.Base;
using SpojDebug.Data.EF.Contexts;
using SpojDebug.Data.Repositories.Result;

namespace SpojDebug.Data.EF.Repositories.Result
{
    public class ResultDetailRepository : Repository<SpojDebugDbContext, ResultDetailEntity>, IResultDetailRepository
    {
        public ResultDetailRepository(SpojDebugDbContext context) : base(context)
        {
        }
    }
}
