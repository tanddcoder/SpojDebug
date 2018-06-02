using System;
using SpojDebug.Core.Entities.Submission;
using System.Collections.Generic;
using SpojDebug.Core.Entities.TestCase;

namespace SpojDebug.Core.Entities.Problem
{
    public class ProblemEntity : BaseEntity<int>
    {
        public int? SpojId { get; set; }
        public string SpojProblemSet { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public float? TimeLimit { get; set; }
        public bool? IsDownloadedTestCase { get; set; }
        public DateTime? DownloadTestCaseTime { get; set; }
        public bool? IsSkip { get; set; }

        public virtual ICollection<TestCaseEntity> Submissions { get; set; }
        public virtual ICollection<TestCaseInfoEntity> TestCaseInfos { get; set; }
    }
}
