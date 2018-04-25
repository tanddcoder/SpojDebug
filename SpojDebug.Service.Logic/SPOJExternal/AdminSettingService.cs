using SpojDebug.Business.AdminSetting;
using SpojDebug.Core.Entities.AdminSetting;
using SpojDebug.Core.Models.AdminSetting;
using SpojDebug.Core.Models.ApplicationResponse;
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

        public ApplicationResult<AdminSettingResponseModel> GetSpojAccount()
        {
            var response = _spojBusiness.GetSpojAccount();

            return ApplicationResult<AdminSettingResponseModel>.Ok(response);
        }

        public void GetSpojInfo()
        {
            _spojBusiness.GetSpojInfo();
        }

        public ApplicationResult UpdateSpojAccount()
        {
            var response = _spojBusiness.UpdateSpojAccount();

            return ApplicationResult.Ok();
        }
    }
}
