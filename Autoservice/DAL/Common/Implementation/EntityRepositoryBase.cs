using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Mehdime.Entity;
using Autoservice.DAL.Common.Interfaces;

namespace Autoservice.DAL.Common.Implementation
{
    public class EntityRepositoryBase<TEntity, TContext> : IEntityRepository<TEntity>
        where TEntity : class, IEntity
        where TContext : DbContext
    {
        private readonly IAmbientDbContextLocator _ambientDbContextLocator;

        protected TContext DbContext
        {
            get
            {
                var dbContext = _ambientDbContextLocator.Get<TContext>();

                if (dbContext == null)
                    throw new InvalidOperationException("No ambient DbContext of requested type found. This means that this repository method has been called outside of the scope of a DbContextScope. A repository must only be accessed within the scope of a DbContextScope, which takes care of creating the DbContext instances that the repositories need and making them available as ambient contexts. This is what ensures that, for any given DbContext-derived type, the same instance is used throughout the duration of a business transaction. To fix this issue, use IDbContextScopeFactory in your top-level business logic service method to create a DbContextScope that wraps the entire business transaction that your service method implements. Then access this repository within that scope. Refer to the comments in the IDbContextScope.cs file for more details.");

                return dbContext;
            }
        }

        public bool Exists(TEntity entity)
        {
            return Entities.Local.Any(e => e == entity);
        }

        public TEntity ExistsDB(TEntity entity)
        {
            return Entities.SingleOrDefault(e => e.Id == entity.Id);
        }

        public EntityRepositoryBase(IAmbientDbContextLocator ambientDbContextLocator)
        {
            if (ambientDbContextLocator == null) throw new ArgumentNullException(nameof(ambientDbContextLocator));
            _ambientDbContextLocator = ambientDbContextLocator;
        }

        protected virtual DbSet<TEntity> Entities => DbContext.Set<TEntity>();


        public virtual void Add(TEntity entity)
        {
            Entities.Add(entity);
        }

        public virtual void Update(TEntity entity)
        {
            if (!Exists(entity))
            {
                Entities.Attach(entity);
            }
            DbContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual void AddOrUpdate(TEntity entity)
        {
            var baseEntity = ExistsDB(entity);
            if (baseEntity == null)
            {
                Entities.Add(entity);
            } else 
                DbContext.Entry(baseEntity).CurrentValues.SetValues(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            Entities.Remove(entity);
        }

        public virtual void Delete(Expression<Func<TEntity, bool>> filter)
        {
            IEnumerable<TEntity> objects = Entities.Where(filter).AsEnumerable();
            foreach (TEntity obj in objects)
                Entities.Remove(obj);
        }

        public virtual TEntity FindById(object id)
        {
            return Entities.Find(id);
        }
        
        public virtual TEntity Get(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includes)
        {
            if (!includes.Any()) return Entities.FirstOrDefault(filter);
            IQueryable<TEntity> get = Entities;
            foreach (var include in includes)
            {
                get = get.Include(include);
            }
            return get.FirstOrDefault(filter);
        }

        public virtual TEntity GetReadOnly(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includes)
        {
            if (!includes.Any()) return Entities.AsNoTracking().FirstOrDefault(filter);
            IQueryable<TEntity> get = Entities;
            foreach (var include in includes)
            {
                get = get.Include(include);
            }
            return get.AsNoTracking().FirstOrDefault(filter);
        }

        public List<TEntity> GetMany(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> get = Entities;
            foreach (var include in includes)
            {
                get = get.Include(include);
            }

            return get.Where(filter).ToList();
        }

        public virtual List<TEntity> GetAll(params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> get = Entities;
            foreach (var include in includes)
            {
                get = get.Include(include);
            }
            return get.ToList();
        }

        public virtual List<TEntity> GetAllReadOnly(params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> get = Entities;
            foreach (var include in includes)
            {
                get = get.Include(include);
            }
            return get.AsNoTracking().ToList();
        }

        /// <summary>
        /// Gets the read only records only.
        /// </summary>
        /// <returns>List of read-only records</returns>
        public virtual IQueryable<TEntity> GetAllReadOnly()
        {
            return Entities.AsNoTracking();
        }

        public virtual int Count()
        {
            return Entities.Count();
        }

        public int Count(Expression<Func<TEntity, bool>> filter)
        {
            return Entities.Where(filter).Count();
        }

        public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> filter)
        {
            return Entities.Where(filter).AsQueryable();
        }

        public IQueryable<TEntity> QueryAll()
        {
            return Entities.AsQueryable();
        }

        public IQueryable<TEntity> GetRangeByIndex<TKey>(int from, int count, Func<TEntity, TKey> order)
        {
            return Entities.OrderBy(order).Skip(from).Take(count).AsQueryable();
        }
    }
}