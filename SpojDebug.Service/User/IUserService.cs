using System.Security.Claims;
using System.Threading.Tasks;
using SpojDebug.Core.Models.Account;
using SpojDebug.Core.Models.ApplicationResponse;
using SpojDebug.Core.User;

namespace SpojDebug.Service.User
{
    public interface IUserService
    {
        ApplicationResult<ApplicationUser> GetCurrentUser(ClaimsPrincipal user);

        SpojAccountModel GetCurrentUserSpojAccount(ClaimsPrincipal user);
        Task UpdateUserSpojAccount(SpojAccountModel model);
    }
}
