using SpojDebug.Business.AdminSetting;
using SpojDebug.Service.SPOJExternal;

namespace SpojDebug.Service.Logic.SPOJExternal
{
    public class SpojExternalService : ISpojExternalService
    {
        private readonly IAdminSettingBusiness _spojBusiness;

        public SpojExternalService(IAdminSettingBusiness spojBusiness)
        {
            _spojBusiness = spojBusiness;
        }

        public void GetSpojInfo()
        {
            _spojBusiness.GetSpojInfo();
        }
    }
}
