namespace SpojDebug.Core.Models.TestCase
{
    public class TestCaseResponseModel
    {
        public int SubmissionId { get; set; }
        public string ProblemCode { get; set; }
        public int TestCaseSeq { get; set; }
        public string ResultName { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
    }
}
