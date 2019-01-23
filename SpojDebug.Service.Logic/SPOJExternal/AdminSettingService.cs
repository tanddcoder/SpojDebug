using System.Threading.Tasks;
using SpojDebug.Business.AdminSetting;
using SpojDebug.Business.Cache;
using SpojDebug.Core.Entities.AdminSetting;
using SpojDebug.Core.Models.AdminSetting;
using SpojDebug.Core.Models.ApplicationResponse;
using SpojDebug.Service.Logic.Base;
using SpojDebug.Service.SPOJExternal;

namespace SpojDebug.Service.Logic.AdminSetting
{
    public class AdminSettingService : Service<IAdminSettingBusiness, AdminSettingEntity>, IAdminSettingService
    {
        private readonly IAdminSettingBusiness _adminSettingBusiness;
        private readonly IAdminSettingCacheBusiness _adminSettingCacheBusiness;

        public AdminSettingService(IAdminSettingBusiness spojBusiness, IAdminSettingCacheBusiness adminSettingCacheBusiness)
        {
            _adminSettingBusiness = spojBusiness;
            _adminSettingCacheBusiness = adminSettingCacheBusiness;
        }

        public async Task<ApplicationResult<AdminSettingSpojAccountResponseModel>> GetSpojAccountAsync()
        {
            var response = await _adminSettingBusiness.GetSpojAccountAsync();

            return ApplicationResult<AdminSettingSpojAccountResponseModel>.Ok(response);
        }

        public void GetSpojInfo()
        {
            _adminSettingBusiness.GetSpojInfo();
        }

        public ApplicationResult UpdateSpojAccount(AdminSettingSpojAccountUpdateModel model)
        {
            _adminSettingBusiness.UpdateSpojAccount(model);

            return ApplicationResult.Ok();
        }

        public void DownloadSpojTestCases()
        {
            _adminSettingBusiness.DownloadSpojTestCases();
        }

        public void GetSubmissionInfo()
        {
            _adminSettingBusiness.GetSubmissionInfo();
        }

        public async Task<AdminSettingModel> GetAdminSetting()
        {
            var response = await _adminSettingCacheBusiness.GetCache();

            return response;
        }

        public async Task<AdminSettingModel> UpdateAdminSetting(AdminSettingUpdateModel model)
        {
            var response = await _adminSettingBusiness.UpdateAdminSetting(model);

            return response;
        }
    }
}
