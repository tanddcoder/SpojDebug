using System;
using System.Collections.Generic;

namespace SpojDebug.Core.Models.SpojModels
{
    public class SpojContestModel
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Name { get; set; }
        public Dictionary<int, SpojProblemInfoModel> ProblemsInfo { get; set; }
        public Dictionary<int, SpojUserModel> Users { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;
}
}
