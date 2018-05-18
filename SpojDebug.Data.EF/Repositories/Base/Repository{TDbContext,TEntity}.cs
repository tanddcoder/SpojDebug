using Microsoft.EntityFrameworkCore;
using SpojDebug.Data.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using SpojDebug.Core.AppSetting;
using SpojDebug.Core.Entities;
using SpojDebug.Ultil.Logger;

namespace SpojDebug.Data.EF.Base
{
    public abstract class Repository<TDbContext,TEntity> : IRepository<TEntity> 
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
            return DbSet.Add(entity).Entity;
        }

        public virtual void Delete(object id)
        {
            var entityToDelete = DbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                DbSet.Attach(entityToDelete);
            }
            DbSet.Remove(entityToDelete);
        }

        public virtual bool TryToUpdate(TEntity entityToUpdate)
        {
            TryAttach(entityToUpdate);
            try
            {
                Context.Entry(entityToUpdate).State = EntityState.Modified;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public int TryToSaveChanges()
        {
            try
            {
                return Context.SaveChanges();
            }
            catch (Exception e)
            {
                //write log here

                return 0;
            }
        }

        private bool TryAttach(TEntity entity)
        {
            try
            {
                if (Context.Entry(entity).State == EntityState.Detached)
                    DbSet.Attach(entity);
                return true;
            }
            catch (Exception e)
            {
                //Writelog here
                LogHepler.WriteErrorLog(e.Message, SystemInfo.ErrorLogFilePath);
                return false;
            }
        }


    }
}
