using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using SpojDebug.Core.Entities;

namespace SpojDebug.Data.Base
{
    public interface IRepository<TEntity> where TEntity : BaseEntity<int>
    {
        IQueryable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null);

        TEntity GetSingle(
            Expression<Func<TEntity, bool>> filter = null);

        TEntity GetById(object id);

        bool Insert(TEntity entity);

        void Remove(TEntity entityToDelete);

        bool TryToUpdate(TEntity entityToUpdate);

        int TryToSaveChanges();
    }
}
