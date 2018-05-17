using AutoMapper;
using SpojDebug.Business.Base;
using SpojDebug.Core.Entities;
using SpojDebug.Data.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SpojDebug.Business.Logic.Base
{
    public abstract class Business<TRepository, TEntity> : IBusiness<TEntity>
        where TEntity : BaseEntity<int>
        where TRepository: IRepository<TEntity>
    {
        protected readonly TRepository Repository;
        private readonly IMapper _mapper;

        protected Business(TRepository repository, IMapper mapper)
        {
            Repository = repository;
            _mapper = mapper;
        }

        public virtual List<TModel> Get<TModel>(Expression<Func<TEntity, bool>> expression, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includeProperties) where TModel : class
        {
            var query = Repository.Get(expression);
            var result = _mapper.Map<List<TModel>>(query.ToList());
            return result;
        }
    }
}
