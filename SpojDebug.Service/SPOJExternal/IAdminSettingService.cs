
using System.Threading.Tasks;
using SpojDebug.Core.Entities.AdminSetting;
using SpojDebug.Core.Models.AdminSetting;
using SpojDebug.Core.Models.ApplicationResponse;
using SpojDebug.Service.Base;

namespace SpojDebug.Service.SPOJExternal
{
    public interface IAdminSettingService : IService<AdminSettingEntity>
    {
        void GetSpojInfo();

        Task<ApplicationResult<AdminSettingSpojAccountResponseModel>> GetSpojAccountAsync();

        ApplicationResult UpdateSpojAccount(AdminSettingSpojAccountUpdateModel model);

        void DownloadSpojTestCases();

        void GetSubmissionInfo();

        Task<AdminSettingModel> GetAdminSetting();

        Task<AdminSettingModel> UpdateAdminSetting(AdminSettingUpdateModel model);
    }
}
