namespace SpojDebug.Core.Entities.Problem
{
    public class ProblemEntity : BaseEntity<int>
    {
        public int? SpojId { get; set; }
        public string SpojCode { get; set; }
        public string SpojLink { get; set; }
    }
}
