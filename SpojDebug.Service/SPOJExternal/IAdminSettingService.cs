
using SpojDebug.Core.Entities.AdminSetting;
using SpojDebug.Core.Models.AdminSetting;
using SpojDebug.Core.Models.ApplicationResponse;
using SpojDebug.Service.Base;

namespace SpojDebug.Service.SPOJExternal
{
    public interface IAdminSettingService : IService<AdminSettingEntity>
    {
        void GetSpojInfo();

        ApplicationResult<AdminSettingSpojAccountResponseModel> GetSpojAccount();

        ApplicationResult UpdateSpojAccount(AdminSettingSpojAccountUpdateModel model);

        void DownloadSpojTestCases();
    }
}
