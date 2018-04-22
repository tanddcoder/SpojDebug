using SpojDebug.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SpojDebug.Business.Base
{
    public interface IBusiness<TEntity> where TEntity : BaseEntity<int>
    {
        List<TModel> Get<TModel>(Expression<Func<TEntity, bool>> expression, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includeProperties) where TModel : class;


    }
}
