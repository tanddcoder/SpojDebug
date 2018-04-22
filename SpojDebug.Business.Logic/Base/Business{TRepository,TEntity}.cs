using SpojDebug.Core.Entities;
using SpojDebug.Data.Base;

namespace SpojDebug.Business.Logic.Base
{
    public abstract class Business<TRepository, TEntity> 
        where TEntity : BaseEntity<int>
        where TRepository: IRepository<TEntity>
    {
        protected readonly TRepository _repository;

        protected Business(TRepository repository)
        {
            _repository = repository;
        } 
    }
}
