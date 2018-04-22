using AutoMapper;
using SpojDebug.Business.Account;
using SpojDebug.Business.Logic.Base;
using SpojDebug.Core.Entities.Account;
using SpojDebug.Data.Repositories.Account;

namespace SpojDebug.Business.Logic.Account
{
    public class AccountBusiness : Business<IAccountRepository, AccountEntity>, IAccountBusiness
    {
        protected AccountBusiness(IAccountRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
