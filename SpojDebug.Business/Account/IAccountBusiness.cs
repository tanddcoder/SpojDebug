using System.Threading.Tasks;
using SpojDebug.Business.Base;
using SpojDebug.Core.Entities.Account;

namespace SpojDebug.Business.Account
{
    public interface IAccountBusiness : IBusiness<AccountEntity>
    {
        Task<(int,string)> GetSpojAccountUsernameAsync(string userId);
    }
}
