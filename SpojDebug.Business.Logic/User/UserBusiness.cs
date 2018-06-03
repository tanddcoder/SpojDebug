﻿using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SpojDebug.Business.User;
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

        public SpojAccountModel GetCurrentUserSpojAccount(ClaimsPrincipal user)
        {
            var userId = _userRepository.GetUserId(user);

            var account = _userRepository.Get(x => x.Id == userId).Include(x => x.Accounts).SelectMany(x => x.Accounts).FirstOrDefault();

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

        public async Task UpdateUSerSpojAccountAsync(SpojAccountModel model)
        {
            if (model.Password != model.ConfirmPassword)
                throw new SpojDebugException("Password and confirm password must equal");

            var account = _accountRepository.Get(x => x.UserName == model.Username).FirstOrDefault();

            if (account == null)
                throw new SpojDebugException("Spoj Account has not existed in system");

            if(account.UserId != null)
                throw new SpojDebugException("Sorry. This Spoj account was used by another user");
            using (var client = new SpojClient())
            {
                await client.LoginAsync(model.Username, model.Password);

                var isLogin = await client.IsLoginSuccess();

                if (!isLogin)
                    throw new SpojDebugException("Sorry. We can not authorized this Spoj account for you. Make sure that you can login this account into Spoj.com");

                account.UserId = model.UserId;

                _accountRepository.Update(account, x => x.UserId);
                _accountRepository.SaveChanges();
            }
        }
    }
}