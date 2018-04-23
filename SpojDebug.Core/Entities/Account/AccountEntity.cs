using SpojDebug.Core.Constant;
using SpojDebug.Core.Entities.Submission;
using SpojDebug.Core.User;
using System.Collections.Generic;

namespace SpojDebug.Core.Entities.Account
{
    public class AccountEntity : BaseEntity<int>
    {
        public string UserId { get; set; }
        public string SpojUserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Enums.SocialNetworkType SocialNetWorkType { get; set; }

        public List<SubmissionEntity> Submissions { get; set; }
        public ApplicationUser User { get; set; }
    }
}
