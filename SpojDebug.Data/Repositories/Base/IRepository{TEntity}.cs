using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SpojDebug.Data.Base
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null);

        TEntity GetSingle(
            Expression<Func<TEntity, bool>> filter = null);

        TEntity GetById(object id);

        TEntity Insert(TEntity entity);

        void Delete(TEntity entityToDelete);

        bool TryToUpdate(TEntity entityToUpdate);

        int SaveChanges();
    }
}
