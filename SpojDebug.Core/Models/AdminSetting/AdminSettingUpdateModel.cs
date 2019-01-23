namespace SpojDebug.Core.Models.AdminSetting
{
    public class AdminSettingUpdateModel : AdminSettingModel
    {
        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }

        public string OldEmailPassword { get; set; }

        public string NewEmailPassword { get; set; }

        public string ConfirmEmailPassword { get; set; }
    }
}
