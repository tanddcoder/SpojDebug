using SpojDebug.Core.Entities.Problem;

namespace SpojDebug.Core.Entities.TestCase
{
    public class TestCaseEntity : BaseEntity<int>
    {
        public int ProblemId { get; set; }
        public int SeqNum { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }

        public ProblemEntity Problem { get; set; }
    }
}
