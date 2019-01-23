using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using SpojDebug.Business.Cache;
using SpojDebug.Core.AppSetting;
using SpojDebug.Core.Models.AdminSetting;
using SpojDebug.Data.Repositories.AdminSetting;
using SpojDebug.Ultil.DataSecurity;

namespace SpojDebug.Business.Logic.Cache
{
    public class AdminSettingCacheBusiness : IAdminSettingCacheBusiness
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IAdminSettingRepository _adminSettingRepository;
        private const string CacheKey = "AdminCache";

        public AdminSettingCacheBusiness(IMemoryCache memoryCache, IAdminSettingRepository adminSettingRepository)
        {
            _memoryCache = memoryCache;
            _adminSettingRepository = adminSettingRepository;
        }

        public Task<AdminSettingModel> GetCache()
        {
            _memoryCache.TryGetValue(CacheKey, out AdminSettingCacheModel raw);
            if(raw == null)
            {
                raw = SetToCache();
            }

            var model = new AdminSettingModel
            {
                TestCaseLimitation = raw.TestCaseLimitation,
                UserName = (DataSecurityUltils.Decrypt(raw.UserName, ApplicationConfigs.SpojKey.ForUserName)),
                Id = raw.Id,
                ContestName = raw.ContestName,
                SystemEmail = raw.SystemEmail
            };


            return Task.FromResult(model);
        }

        public void RemoveCache()
        {
            _memoryCache.Remove(CacheKey);
        }

        public Task<AdminAccountModel> GetAdminAccount()
        {
            _memoryCache.TryGetValue(CacheKey, out AdminSettingCacheModel raw);
            if (raw == null)
            {
                raw = SetToCache();
            }
            var model = new AdminAccountModel
            {
                Id = raw.Id,
                Password = (DataSecurityUltils.Decrypt(raw.Password, ApplicationConfigs.SpojKey.ForPassword)),
                Username = (DataSecurityUltils.Decrypt(raw.UserName, ApplicationConfigs.SpojKey.ForUserName))
            };


            return Task.FromResult(model);
        }

        private AdminSettingCacheModel SetToCache()
        {
            var raw = _adminSettingRepository.Get().Select(x => new AdminSettingCacheModel
            {
                Id = x.Id,
                Password = x.SpojPasswordEncode,
                TestCaseLimitation = x.TestCaseLimit ?? 0,
                UserName = x.SpojUserNameEncode,
                ContestName = x.ContestName,
                SystemEmail = x.SystemEmail,
                SystemEmailPasswordEncode = x.SystemEmailPasswordEncode
            }).FirstOrDefault();
            _memoryCache.Set(CacheKey, raw);

            return raw;
        }

        public Task<AdminSystemEmailInfoModel> GetEmailInfo()
        {
            _memoryCache.TryGetValue(CacheKey, out AdminSettingCacheModel raw);

            if (raw == null)
            {
                raw = SetToCache();
            }
            var model = new AdminSystemEmailInfoModel
            {
                Id = raw.Id,
                Password = (DataSecurityUltils.Decrypt(raw.SystemEmailPasswordEncode, ApplicationConfigs.SpojKey.ForPassword)),
                Email = raw.SystemEmail
            };

            return Task.FromResult(model);
        }
    }
}
