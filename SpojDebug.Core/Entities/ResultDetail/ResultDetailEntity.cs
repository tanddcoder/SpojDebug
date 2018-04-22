using SpojDebug.Core.Constant;
using SpojDebug.Core.Entities.Result;
using SpojDebug.Core.Entities.TestCase;

namespace SpojDebug.Core.Entities.ResultDetail
{
    public class ResultDetailEntity : BaseEntity<int>
    {
        public int ResultId { get; set; }
        public int TestCaseId { get; set; }
        public Enums.ResultType ResultType { get; set; }

        public TestCaseEntity TestCase { get; set; }
        public ResultEntity Result { get; set; }

    }
}
