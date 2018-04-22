using SpojDebug.Core.Constant;
using SpojDebug.Core.Entities.Submission;
using System.Collections.Generic;

namespace SpojDebug.Core.Entities.Account
{
    public class AccountEntity : BaseEntity<int>
    {
        public string SpojUserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Enums.SocialNetworkType SocialNetWorkType { get; set; }

        List<SubmissionEntity> Submissions { get; set; }
    }
}
