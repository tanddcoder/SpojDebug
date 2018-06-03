using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using SpojDebug.Core.Entities;

namespace SpojDebug.Data.Base
{
    public interface IRepository<TEntity> : IDisposable where TEntity : BaseEntity<int>
    {
        IQueryable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null, bool isIncludeDeleted = false);

        TEntity GetSingle(
            Expression<Func<TEntity, bool>> filter = null, bool isIncludeDeleted = false);

        TEntity GetById(object id);

        TEntity Insert(TEntity entity);

        void Remove(TEntity entityToDelete);

        void Update(TEntity entity, params Expression<Func<TEntity, object>>[] changedProperties);

        int SaveChanges();

        void InsertRange(IEnumerable<TEntity> entities);
    }
}
