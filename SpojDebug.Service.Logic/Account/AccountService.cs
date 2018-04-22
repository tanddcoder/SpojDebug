using SpojDebug.Business.Account;
using SpojDebug.Core.Entities.Account;
using SpojDebug.Service.Account;
using SpojDebug.Service.Logic.Base;

namespace SpojDebug.Service.Logic.Account
{
    public class AccountService : Service<IAccountBusiness,AccountEntity>, IAccountService
    {
        
    }
}
