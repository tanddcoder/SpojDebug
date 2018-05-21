using Microsoft.EntityFrameworkCore;
using SpojDebug.Data.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SpojDebug.Core.AppSetting;
using SpojDebug.Core.Entities;
using SpojDebug.Ultil.Logger;

namespace SpojDebug.Data.EF.Base
{
    public abstract class Repository<TDbContext, TEntity> : IRepository<TEntity>
        where TDbContext : DbContext
        where TEntity : BaseEntity<int>
    {
        protected readonly TDbContext Context;
        protected readonly DbSet<TEntity> DbSet;
        protected readonly SystemInfo SystemInfo;

        protected Repository(TDbContext context, SystemInfo systemInfo)
        {
            this.Context = context;
            this.DbSet = context.Set<TEntity>();
            SystemInfo = systemInfo;
        }

        public virtual IQueryable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null)
        {
            var query = DbSet.AsNoTracking();

            query = query.Where(x => x.DeletedTime == null);

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query;
        }

        public TEntity GetSingle(
            Expression<Func<TEntity, bool>> filter = null)
        {
            var query = DbSet.AsNoTracking();

            query = query.Where(x => x.DeletedTime == null);

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.FirstOrDefault();

        }

        public virtual TEntity GetById(object id)
        {
            return DbSet.Find(id);
        }

        public virtual TEntity Insert(TEntity entity)
        {
            entity.DeletedTime = null;
            entity.LastUpdatedTime = null;

            entity.CreatedTime = DateTime.Now;

            entity = DbSet.Add(entity).Entity;
            return entity;
        }

        public virtual void InsertRange(IEnumerable<TEntity> entities)
        {
            var enumerable = entities as TEntity[] ?? entities.ToArray();
            foreach (var entity in enumerable)
            {
                Insert(entity);
            }
        }

        public virtual void Delete(TEntity entity)
        {
            entity.DeletedTime = DateTime.Now;
            Update(entity);
        }

        public virtual void Remove(object id)
        {
            var entityToDelete = DbSet.Find(id);
            Remove(entityToDelete);
        }

        public virtual void Remove(TEntity entityToDelete)
        {
            if (Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                DbSet.Attach(entityToDelete);
            }
            DbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entity, params Expression<Func<TEntity, object>>[] changedProperties)
        {
            TryAttach(entity);
            changedProperties = changedProperties?.Distinct().ToArray();

            if (changedProperties?.Any() == true)
            {
                foreach (var property in changedProperties)
                {
                    Context.Entry(entity).Property(property).IsModified = true;
                }
            }
            else
                Context.Entry(entity).State = EntityState.Modified;
        }

        public int SaveChanges()
        {
            return Context.SaveChanges();
        }

        private bool TryAttach(TEntity entity)
        {
            try
            {
                if (Context.Entry(entity).State == EntityState.Detached)
                    DbSet.Attach(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}
