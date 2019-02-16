using System.Security.Claims;
using System.Threading.Tasks;
using SpojDebug.Core.Models.Account;
using SpojDebug.Core.Models.User;

namespace SpojDebug.Business.User
{
    public interface IUserBusiness
    {
        Task<SpojAccountModel> GetCurrentUserSpojAccountAsync(ClaimsPrincipal user);
        Task UpdateUSerSpojAccountAsyncAsync(SpojAccountModel model);
        Task DeleteSpojAccount(string userId);
        UserInfoModel GetCurrentUser(ClaimsPrincipal user);
    }
}
