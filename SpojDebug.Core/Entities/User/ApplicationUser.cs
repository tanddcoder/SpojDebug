using Microsoft.AspNetCore.Identity;
using SpojDebug.Core.Entities.Account;
using System.Collections.Generic;

namespace SpojDebug.Core.User
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<AccountEntity> Accounts { get; set; }
    }
}
