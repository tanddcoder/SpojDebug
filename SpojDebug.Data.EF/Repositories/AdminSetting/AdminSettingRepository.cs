using SpojDebug.Core.AppSetting;
using SpojDebug.Core.Entities.AdminSetting;
using SpojDebug.Data.EF.Base;
using SpojDebug.Data.EF.Contexts;
using SpojDebug.Data.Repositories.AdminSetting;

namespace SpojDebug.Data.EF.Repositories.AdminSetting
{
    public class AdminSettingRepository : Repository<SpojDebugDbContext, AdminSettingEntity>, IAdminSettingRepository
    {
        public AdminSettingRepository(SpojDebugDbContext context) : base(context)
        {
        }
    }
}
