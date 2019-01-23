using System.Threading.Tasks;
using SpojDebug.Business.Base;
using SpojDebug.Core.Entities.AdminSetting;
using SpojDebug.Core.Models.AdminSetting;

namespace SpojDebug.Business.AdminSetting
{
    public interface IAdminSettingBusiness : IBusiness<AdminSettingEntity>
    {
        void GetSpojInfo();

        Task<AdminSettingSpojAccountResponseModel> GetSpojAccountAsync();

        void UpdateSpojAccount(AdminSettingSpojAccountUpdateModel model);

        void DownloadSpojTestCases();

        void GetSubmissionInfo();

        Task<AdminSettingModel> UpdateAdminSetting(AdminSettingUpdateModel model);
    }
}
