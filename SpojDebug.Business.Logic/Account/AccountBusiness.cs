using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SpojDebug.Business.Account;
using SpojDebug.Business.Logic.Base;
using SpojDebug.Core.Entities.Account;
using SpojDebug.Data.Repositories.Account;

namespace SpojDebug.Business.Logic.Account
{
    public class AccountBusiness : Business<IAccountRepository, AccountEntity>, IAccountBusiness
    {
        public AccountBusiness(IAccountRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }

        public async Task<(int,string)> GetSpojAccountUsernameAsync(string userId)
        {
            var result = await Repository.Get(x => x.UserId == userId).Select(x => new { x.UserName, x.Id }).FirstOrDefaultAsync();

            return (result.Id, result.UserName);
        }
    }
}
