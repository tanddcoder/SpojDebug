﻿using SpojDebug.Core.Models.AdminSetting;
using System.Threading.Tasks;

namespace SpojDebug.Business.Cache
{
    public interface IAdminSettingCacheBusiness
    {
        Task<AdminSettingModel> GetCache();
        void RemoveCache();

        Task<AdminAccountModel> GetAdminAccount();
        Task<AdminSystemEmailInfoModel> GetEmailInfo();
    }
}
