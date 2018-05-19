namespace SpojDebug.Core.Entities.AdminSetting
{
    public class AdminSettingEntity : BaseEntity<int>
    {
        public string SpojUserNameEncode { get; set; }
        public string SpojPasswordEncode { get; set; }  
    }
}
