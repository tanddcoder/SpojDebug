using System.ComponentModel.DataAnnotations;

namespace SpojDebug.Core.Models.AdminSetting
{
    public class AdminSettingUpdateModel : AdminSettingModel
    {
        [Display(Name = "Update Admin Spoj Account")]
        public bool IsUpdateAccount { get; set; } = false;

        public bool IsNewAccount { get; set; } = false;

        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }

        public bool IsUpdateEmail { get; set; } = false;

        public bool IsNewEmail { get; set; } = false;

        public string OldEmailPassword { get; set; }

        public string NewEmailPassword { get; set; }

        public string ConfirmEmailPassword { get; set; }

        public bool IsUpdateConfig { get; set; } = false;
        public bool Unlimited { get; set; } = false;
    }
}
