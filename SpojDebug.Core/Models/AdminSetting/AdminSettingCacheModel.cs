namespace SpojDebug.Core.Models.AdminSetting
{
    public class AdminSettingCacheModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        
        public string Password { get; set; }

        public int? TestCaseLimitation { get; set; }
        public string ContestName { get; set; }
        public string SystemEmail { get; set; }
        public string SystemEmailPasswordEncode { get; set; }
    }
}
