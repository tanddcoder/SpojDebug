using System;

namespace SpojDebug.Core.Models.SpojModels
{
    public class SpojSubmissionModel
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public float Score { get; set; }
        public float RunTime { get; set; }
        public string Language { get; set; }
    }
}
