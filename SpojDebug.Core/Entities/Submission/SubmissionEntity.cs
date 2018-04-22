using SpojDebug.Core.Entities.Account;
using SpojDebug.Core.Entities.Problem;
using SpojDebug.Core.Entities.Result;

namespace SpojDebug.Core.Entities.Submission
{
    public class SubmissionEntity : BaseEntity<int>
    {
        public int AccountId { get; set; }
        public int ProblemId { get; set; }
        public int SpojId { get; set; }
        public int ResultId { get; set; }

        public AccountEntity Account { get; set; }
        public ProblemEntity Problem { get; set; }
        public ResultEntity Result { get; set; }
    }
}
