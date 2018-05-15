using SpojDebug.Business.Base;
using SpojDebug.Core.Entities.AdminSetting;
using SpojDebug.Core.Models.AdminSetting;

namespace SpojDebug.Business.AdminSetting
{
    public interface IAdminSettingBusiness : IBusiness<AdminSettingEntity>
    {
        void GetSpojInfo();

        AdminSettingSpojAccountResponseModel GetSpojAccount();

        void UpdateSpojAccount(AdminSettingSpojAccountUpdateModel model);

        void DownloadSpojTestCases();
    }
}
