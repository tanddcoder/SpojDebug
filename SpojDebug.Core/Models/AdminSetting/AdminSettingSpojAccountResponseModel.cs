﻿using System.ComponentModel.DataAnnotations;

namespace SpojDebug.Core.Models.AdminSetting
{
    public class AdminSettingResponseModel
    {
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        public string Password { get; set; } = null;
    }
}
