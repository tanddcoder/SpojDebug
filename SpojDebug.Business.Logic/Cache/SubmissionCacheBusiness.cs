using Microsoft.Extensions.Caching.Memory;
using SpojDebug.Business.Cache;
using SpojDebug.Data.Repositories.Submission;
using System.Collections.Generic;
using System.Linq;

namespace SpojDebug.Business.Logic.Cache
{
    public class SubmissionCacheBusiness : ISubmissionCacheBusiness
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ISubmissionRepository _submissionRepository;
        private const string SubmissionCacheKey = "ListSubmissionSpojIds";

        public SubmissionCacheBusiness(IMemoryCache memoryCache, ISubmissionRepository submissionRepository)
        {
            _memoryCache = memoryCache;
            _submissionRepository = submissionRepository;
        }

        public void AddId(int id)
        {
            HashSet<int> set = GetIds();

            set.Add(id);
            _memoryCache.Set(SubmissionCacheKey, set);
        }

        public void AddRangeIds(List<int> ids)
        {
            HashSet<int> set = GetIds();

            foreach (var id in ids)
            {
                set.Add(id);
            }

            _memoryCache.Set(SubmissionCacheKey, set);
        }

        public HashSet<int> GetIds()
        {
            _memoryCache.TryGetValue(SubmissionCacheKey, out HashSet<int> set);
            if (set == null)
            {
                var listId = _submissionRepository.Get().Select(x => x.SpojId).ToList();
                set = _memoryCache.Set(SubmissionCacheKey, new HashSet<int>(listId));
            }
            return set;
        }

    }
}
