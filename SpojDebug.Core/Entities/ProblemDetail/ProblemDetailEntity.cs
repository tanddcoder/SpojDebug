using SpojDebug.Core.Entities.Problem;

namespace SpojDebug.Core.Entities.ProblemDetail
{
    public class ProblemDetailEntity : BaseEntity<int>
    {
        public float? TimeLimit { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? Type { get; set; }
        public string SpojProblemSet { get; set; }
        public int SpojId { get; set; }

        public ProblemEntity Problem { get; set; }
    }
}
