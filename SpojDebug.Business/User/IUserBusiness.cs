using System.Security.Claims;
using System.Threading.Tasks;
using SpojDebug.Core.Models.Account;

namespace SpojDebug.Business.User
{
    public interface IUserBusiness
    {
        SpojAccountModel GetCurrentUserSpojAccount(ClaimsPrincipal user);
        Task UpdateUSerSpojAccountAsync(SpojAccountModel model);
    }
}
