using SpojDebug.Business.Base;
using SpojDebug.Core.Entities;

namespace SpojDebug.Service.Logic.Base
{
    public class Service<TBusiness,TEntity> 
        where TEntity : BaseEntity<int>
        where TBusiness: IBusiness<TEntity>
    {
        
    }
}
