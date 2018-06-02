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

        protected Repository(TDbContext context)
        {
            this.Context = context;
            this.DbSet = context.Set<TEntity>();
        }

        public virtual IQueryable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null)
        {
            var query = DbSet.AsNoTracking();

            //query = query.Where(x => x.DeletedTime == null);

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

            //query = query.Where(x => x.DeletedTime == null);

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
            //entity.DeletedTime = null;
            //entity.LastUpdatedTime = null;

            entity.CreatedTime = DateTime.Now;

            entity = DbSet.Add(entity).Entity;
            return entity;
        }

        public virtual void InsertRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                //entity.DeletedTime = null;
                //entity.LastUpdatedTime = null;

                entity.CreatedTime = DateTime.Now;
            }
            DbSet.AddRange(entities as TEntity[] ?? entities.ToArray());
        }

        public virtual void Delete(TEntity entity)
        {
            //entity.DeletedTime = DateTime.Now;
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

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Repository() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
