namespace SpojDebug.Core.Models.AdminSetting
{
    public class AdminSettingSpojAccountUpdateModel
    {
        public string UserName { get; set; }

        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmNewPassword { get; set; }
    }
}
