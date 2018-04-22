using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpojDebug.Core.Models.SpojModels
{
    public class SpojProblemModel
    {
        public int Id { get; set; }
        public String Code { get; set; }
        public List<SpojSubmissionModel> Submissions { get; set; } = new List<SpojSubmissionModel>();
    }
}
