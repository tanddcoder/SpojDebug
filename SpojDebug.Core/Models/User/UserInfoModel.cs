using System.Collections.Generic;

namespace SpojDebug.Core.Models.User
{
    public class UserInfoModel
    {
        public List<UserAccount> Accounts { get; set; }
    }

    public class UserAccount
    {
        public int SpojUserId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string UserId { get; set; }
    }
}
