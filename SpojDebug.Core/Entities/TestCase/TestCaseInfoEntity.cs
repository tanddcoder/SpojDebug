using SpojDebug.Core.Entities.Problem;

namespace SpojDebug.Core.Entities.TestCase
{
    public class TestCaseInfoEntity : BaseEntity<int>
    {
        public int ProblemId { get; set; }
        public int TotalTestCase { get; set; }
        public string Path { get; set; }

        public virtual ProblemEntity Problem { get; set; }
    }
}
