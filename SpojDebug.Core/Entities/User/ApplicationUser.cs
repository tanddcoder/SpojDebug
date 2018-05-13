using Microsoft.AspNetCore.Identity;
using SpojDebug.Core.Entities.Account;
using System.Collections.Generic;
using SpojDebug.Core.Entities.Problem;

namespace SpojDebug.Core.User
{
    public class ApplicationUser : IdentityUser
    {
        public List<AccountEntity> Accounts { get; set; }
    }
}
