
using SpojDebug.Core.Entities.AdminSetting;
using SpojDebug.Service.Base;

namespace SpojDebug.Service.SPOJExternal
{
    public interface IAdminSettingService : IService<AdminSettingEntity>
    {
        void GetSpojInfo();
    }
}
