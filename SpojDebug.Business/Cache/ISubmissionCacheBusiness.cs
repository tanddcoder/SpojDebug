using System.Collections.Generic;

namespace SpojDebug.Business.Cache
{
    public interface ISubmissionCacheBusiness
    {
        void AddId(int id);

        void AddRangeIds(List<int> Ids);

        HashSet<int> GetIds();
    }
}
