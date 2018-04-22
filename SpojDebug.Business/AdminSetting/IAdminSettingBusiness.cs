using SpojDebug.Business.Base;
using SpojDebug.Core.Entities.AdminSetting;

namespace SpojDebug.Business.AdminSetting
{
    public interface IAdminSettingBusiness : IBusiness<AdminSettingEntity>
    {
        bool Login();

        void GetSpojInfo();

        void UpdateSetting();
    }
}
