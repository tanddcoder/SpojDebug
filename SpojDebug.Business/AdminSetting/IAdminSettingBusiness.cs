using SpojDebug.Business.Base;
using SpojDebug.Core.Entities.AdminSetting;
using SpojDebug.Core.Models.AdminSetting;

namespace SpojDebug.Business.AdminSetting
{
    public interface IAdminSettingBusiness : IBusiness<AdminSettingEntity>
    {
        void GetSpojInfo();

        void UpdateSetting();

        AdminSettingResponseModel GetSpojAccount();

        bool UpdateSpojAccount();
    }
}
