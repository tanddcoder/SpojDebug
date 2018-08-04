using System.Collections.Generic;

namespace SpojDebug.Business.Cache
{
    public interface IProblemCacheBusiness
    {
        void AddId(int id);

        void AddRangeIds(List<int> Ids);

        HashSet<int> GetIds();

        int GetProblemIdByCode(string code);
    }
}
