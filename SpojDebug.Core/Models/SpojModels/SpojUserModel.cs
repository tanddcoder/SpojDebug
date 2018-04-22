using System.Collections.Generic;

namespace SpojDebug.Core.Models.SpojModels
{
    public class SpojUserModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public Dictionary<int, SpojProblemModel> Problems { get; set; } = new Dictionary<int, SpojProblemModel>();
    }
}
