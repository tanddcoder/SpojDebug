using System.ComponentModel.DataAnnotations;

namespace SpojDebug.Core.Models.Submission
{
    public class SubmissionHomeModel
    {
        [Display(Name = "Id")]
        public int SubmissionId { get; set; }

        [Display(Name = "Problemset")]
        public string ProblemCode { get; set; }

        [Display(Name = "Accepted Test Case")]
        public int AcceptedTestCase { get; set; }

        [Display(Name = "Total Test Case")]
        public int TotalTestCase { get; set; }

        [Display(Name = "First Fail")]
        public int FirtFailTestCase { get; set; }
    }
}
