namespace SpojDebug.Core.Entities.AdminSetting
{
    public class AdminSettingEntity : BaseEntity<int>
    {
        public string SpojUserNameEncode { get; set; }
        public string SpojPasswordEncode { get; set; }
        public int? TestCaseLimit { get; set; }
        public string ContestName { get; set; }
        public string SystemEmail { get; set; }
        public string SystemEmailPasswordEncode { get; set; }
    }
}
