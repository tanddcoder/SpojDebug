using SpojDebug.Core.Models.TestCase;

namespace SpojDebug.Core.Models.Submission
{
    public class SubmissionGetFirstFailModel
    {
        public TestCaseResultSeqPairModel FirstFailTestCase { get; set; }
        public string ProblemCode { get; set; }
        public int SubmissionSpojId { get; set; }
    }
}
