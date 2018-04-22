namespace SpojDebug.Core.Models.SpojModels
{
    public class SpojProblemInfoModel
    {
        public int Id { get; set; }
        public float TimeLimit { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public string ProblemSet { get; set; }
    }
}
