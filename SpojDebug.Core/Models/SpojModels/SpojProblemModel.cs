using System;
using System.Collections.Generic;
using SpojDebug.Core.Entities.Submission;

namespace SpojDebug.Core.Models.SpojModels
{
    public class SpojProblemModel
    {
        public int Id { get; set; }
        public String Code { get; set; }
        public List<SubmissionEntity> Submissions { get; set; } = new List<SubmissionEntity>();
    }
}
