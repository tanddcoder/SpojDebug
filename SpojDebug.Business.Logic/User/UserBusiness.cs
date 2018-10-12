using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SpojDebug.Business.User;
using SpojDebug.Core.Entities.Account;
using SpojDebug.Core.Models.Account;
using SpojDebug.Data.Repositories.Account;
using SpojDebug.Data.Repositories.User;
using SpojDebug.Ultil.Exception;

namespace SpojDebug.Business.Logic.User
{
    public class UserBusiness : IUserBusiness
    {
        private readonly IUserRepository _userRepository;
        private readonly IAccountRepository _accountRepository;

        public UserBusiness(IUserRepository userRepository, IAccountRepository accountRepository)
        {
            _userRepository = userRepository;
            _accountRepository = accountRepository;
        }

        public async Task DeleteSpojAccount(string userId)
        {
            var accounts = await _accountRepository.Get(x => x.UserId == userId).Select(x => new AccountEntity{Id = x.Id, UserId = null}).ToListAsync();

            foreach (var account in accounts)
            {
                _accountRepository.Update(account, x => x.UserId);
            }

            _accountRepository.SaveChanges();
        }

        public async Task<SpojAccountModel> GetCurrentUserSpojAccountAsync(ClaimsPrincipal user)
        {
            var userId = _userRepository.GetUserId(user);

            var account = await _userRepository.Get(x => x.Id == userId).Include(x => x.Accounts).SelectMany(x => x.Accounts).FirstOrDefaultAsync();

            if (account == null)
                return new SpojAccountModel
                {
                    IsUpdated = false
                };
            return new SpojAccountModel
            {
                IsUpdated = true,
                Username = account.UserName
            };
        }

        public async Task UpdateUSerSpojAccountAsyncAsync(SpojAccountModel model)
        {
            if (model.Password != model.ConfirmPassword)
                throw new SpojDebugException("Password and confirm password must equal");

            var account = _accountRepository.Get(x => x.UserName == model.Username).FirstOrDefault();

            if (account == null)
                throw new SpojDebugException("Spoj Account has not existed in system");

            if(account.UserId != null && account.UserId != model.UserId)
                throw new SpojDebugException("Sorry. This Spoj account was used by another user");

            using (var client = new SpojClient())
            {
                await client.LoginAsync(model.Username, model.Password);

                var isLogin = await client.IsLoginSuccess();

                if (!isLogin)
                    throw new SpojDebugException("Sorry. We can not authorized this Spoj account for you. Make sure that you can login this account into Spoj.com");

                account.UserId = model.UserId;

                var oldAccounts = _accountRepository.Get(x => x.UserId == model.UserId);
                foreach (var acc in oldAccounts.ToList())
                {
                    acc.UserId = null;
                    _accountRepository.Update(acc, x => x.UserId);
                }

                _accountRepository.Update(account, x => x.UserId);
                _accountRepository.SaveChanges();
            }
        }
    }
}
