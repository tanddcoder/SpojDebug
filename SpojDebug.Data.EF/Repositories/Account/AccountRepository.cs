using SpojDebug.Core.Entities.Account;
using SpojDebug.Data.EF.Base;
using SpojDebug.Data.EF.Contexts;
using SpojDebug.Data.Repositories.Account;

namespace SpojDebug.Data.EF.Repositories.Account
{
    public class AccountRepository : Repository<SpojDebugDbContext, AccountEntity>, IAccountRepository
    {
        public AccountRepository(SpojDebugDbContext context) : base(context)
        {
        }
    }
}
