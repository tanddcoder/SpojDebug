using SpojDebug.Core.Constant;
using SpojDebug.Core.Entities.Problem;
using SpojDebug.Core.Entities.ResultDetail;
using SpojDebug.Core.Entities.Submission;
using System.Collections.Generic;

namespace SpojDebug.Core.Entities.Result
{
    public class ResultEntity : BaseEntity<int>
    {
        public int ProblemId { get; set; }
        public int SubmmissionId { get; set; }
        public Enums.ResultType? FinalResult { get; set; }
        public int? AcceptedResultCount { get; set; }
        public int? TotalResult { get; set; }

        public SubmissionEntity Submission { get; set; }
        public ProblemEntity Problem { get; set; }
        public List<ResultDetailEntity> ResultDetails { get; set; }
        
    }
}
