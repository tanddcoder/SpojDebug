namespace SpojDebug.Core.Models.Submission
{
    public class SubmissionFirstFailModel
    {
        public int SubmissionId { get; set; }
        public string ProblemCode { get; set; }
        public int TestCaseSeq { get; set; }
        public string ResultName { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
    }
}
