using SpojDebug.Core.Constant;
using SpojDebug.Core.Entities.Submission;
using System.Collections.Generic;

namespace SpojDebug.Core.Entities.Result
{
    public class ResultEntity : BaseEntity<int>
    {
        public int SubmissionId { get; set; }
        public int TestCaseSeq { get; set; }
        public Enums.ResultType Result { get; set; }

        public virtual SubmissionEntity Submission { get; set; }
        
    }
}
