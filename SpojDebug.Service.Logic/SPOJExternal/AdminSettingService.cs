using SpojDebug.Business.AdminSetting;
using SpojDebug.Core.Entities.AdminSetting;
using SpojDebug.Service.Logic.Base;
using SpojDebug.Service.SPOJExternal;

namespace SpojDebug.Service.Logic.AdminSetting
{
    public class AdminSettingService : Service<IAdminSettingBusiness, AdminSettingEntity>, IAdminSettingService
    {
        private readonly IAdminSettingBusiness _spojBusiness;

        public AdminSettingService(IAdminSettingBusiness spojBusiness)
        {
            _spojBusiness = spojBusiness;
        }

        public void GetSpojInfo()
        {
            _spojBusiness.GetSpojInfo();
        }
    }
}
