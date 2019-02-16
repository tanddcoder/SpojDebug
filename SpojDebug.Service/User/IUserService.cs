using System.Security.Claims;
using System.Threading.Tasks;
using SpojDebug.Core.Models.Account;
using SpojDebug.Core.Models.User;

namespace SpojDebug.Service.User
{
    public interface IUserService
    {
        UserInfoModel GetCurrentUser(ClaimsPrincipal user);

        Task<SpojAccountModel> GetCurrentUserSpojAccountAsync(ClaimsPrincipal user);
        Task UpdateUserSpojAccountAsync(SpojAccountModel model);
        Task DeleteSpojAccount(string userId);
    }
}
