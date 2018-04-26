using SpojDebug.Core.Entities.Account;
using SpojDebug.Core.Entities.Problem;
using SpojDebug.Core.Entities.Result;
using System;

namespace SpojDebug.Core.Entities.Submission
{
    public class SubmissionEntity : BaseEntity<int>
    {
        public int AccountId { get; set; }
        public int ProblemId { get; set; }
        public int SpojId { get; set; }
        public int ResultId { get; set; }
        public DateTime SubmitTime { get; set; }
        public float Score { get; set; }
        public float RunTime { get; set; }
        public string Language { get; set; }

        public AccountEntity Account { get; set; }
        public ProblemEntity Problem { get; set; }
        public ResultEntity Result { get; set; }
    }
}
