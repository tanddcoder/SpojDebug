using SpojDebug.Core.Entities.Account;
using SpojDebug.Core.Entities.Problem;
using SpojDebug.Core.Entities.Result;
using System;
using System.Collections.Generic;

namespace SpojDebug.Core.Entities.Submission
{
    public class SubmissionEntity : BaseEntity<int>
    {
        public int? AccountId { get; set; }
        public int? ProblemId { get; set; }
        public int SpojId { get; set; }
        public DateTime SubmitTime { get; set; }
        public float Score { get; set; }
        public float RunTime { get; set; }
        public string Language { get; set; }
        public bool? IsDownloadedInfo { get; set; }
        public DateTime? DownloadedTime { get; set; }
        public bool? IsNotHaveEnoughInfo { get; set; }
        public int? TotalResult { get; set; }

        public virtual AccountEntity Account { get; set; }
        public virtual ProblemEntity Problem { get; set; }
        public virtual ICollection<ResultEntity> Results { get; set; }
    }
}
