using System.Security.Claims;
using System.Threading.Tasks;
using SpojDebug.Business.User;
using SpojDebug.Core.Models.Account;
using SpojDebug.Core.Models.User;
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

        public async Task DeleteSpojAccount(string userId)
        {
            await _userBusiness.DeleteSpojAccount(userId);
        }

        public UserInfoModel GetCurrentUser(ClaimsPrincipal user)
        {
            return _userBusiness.GetCurrentUser(user);
        }

        public async Task<SpojAccountModel> GetCurrentUserSpojAccountAsync(ClaimsPrincipal user)
        {

            return await _userBusiness.GetCurrentUserSpojAccountAsync(user);
        }

        public async Task UpdateUserSpojAccountAsync(SpojAccountModel model)
        {
            await _userBusiness.UpdateUSerSpojAccountAsyncAsync(model);
        }
    }
}
