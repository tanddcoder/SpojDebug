using System;
using System.Security.Claims;
using System.Threading.Tasks;
using SpojDebug.Business.User;
using SpojDebug.Core.Models.Account;
using SpojDebug.Core.Models.ApplicationResponse;
using SpojDebug.Core.User;
using SpojDebug.Service.User;

namespace SpojDebug.Service.Logic.User
{
    public class UserService : IUserService
    {
        private readonly IUserBusiness _userBusiness;

        public UserService(IUserBusiness userBusiness)
        {
            _userBusiness = userBusiness;
        }


        public ApplicationResult<ApplicationUser> GetCurrentUser(ClaimsPrincipal user)
        {
            throw new NotImplementedException();
        }

        public SpojAccountModel GetCurrentUserSpojAccount(ClaimsPrincipal user)
        {

            return _userBusiness.GetCurrentUserSpojAccount(user);
        }

        public async Task UpdateUserSpojAccount(SpojAccountModel model)
        {
            await _userBusiness.UpdateUSerSpojAccountAsync(model);
        }
    }
}
