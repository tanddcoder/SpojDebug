using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SpojDebug.Data.Base
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, object>>[] includeProperties);

        TEntity GetSingle(
            Expression<Func<TEntity, bool>> filter = null,
            params Expression<Func<TEntity, object>>[] includeProperties);

        TEntity GetByID(object id);

        TEntity Insert(TEntity entity);

        void Delete(TEntity entityToDelete);

        void Update(TEntity entityToUpdate);

        int SaveChanges();
    }
}
