using Microsoft.Extensions.Caching.Memory;
using SpojDebug.Business.Cache;
using SpojDebug.Data.Repositories.Problem;
using System.Collections.Generic;
using System.Linq;

namespace SpojDebug.Business.Logic.Cache
{
    public class ProblemCacheBusiness : IProblemCacheBusiness
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IProblemRepository _problemRepository;
        private const string ProblemCacheKey = "ListProblemSpojIds";

        public ProblemCacheBusiness(IMemoryCache memoryCache, IProblemRepository problemRepository)
        {
            _memoryCache = memoryCache;
            _problemRepository = problemRepository;
        }

        public void AddId(int id)
        {
            HashSet<int> set = GetIds();

            set.Add(id);
            _memoryCache.Set(ProblemCacheKey, set);
        }

        public void AddRangeIds(List<int> ids)
        {
            HashSet<int> set = GetIds();

            foreach (var id in ids)
            {
                set.Add(id);
            }

            _memoryCache.Set(ProblemCacheKey, set);
        }

        public HashSet<int> GetIds()
        {
            _memoryCache.TryGetValue(ProblemCacheKey, out HashSet<int> set);
            if (set == null)
            {
                var listId = _problemRepository.Get().Select(x => x.SpojId.Value).ToList();
                set = _memoryCache.Set(ProblemCacheKey, new HashSet<int>(listId));
            }
            return set;
        }

        public int GetProblemIdByCode(string code)
        {
            _memoryCache.TryGetValue(code, out int id);
            if (id <= 0)
            {
                var idSelected = _problemRepository.Get(x => x.Code == code).Select(x => x.Id).FirstOrDefault();
                id = _memoryCache.Set(code, idSelected);
            }
            return id;
        }
    }
}
