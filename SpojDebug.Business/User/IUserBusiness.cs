using System.Security.Claims;
using System.Threading.Tasks;
using SpojDebug.Core.Models.Account;

namespace SpojDebug.Business.User
{
    public interface IUserBusiness
    {
        Task<SpojAccountModel> GetCurrentUserSpojAccountAsync(ClaimsPrincipal user);
        Task UpdateUSerSpojAccountAsyncAsync(SpojAccountModel model);
    }
}
