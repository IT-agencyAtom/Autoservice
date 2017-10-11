using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Autoservice.DAL.Common.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для сущности TEntity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IEntityRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Adds the specified entity to database context.
        /// </summary>
        /// <param name="entity">The entity to be added.</param>
        void Add(TEntity entity);

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        void Update(TEntity entity);

        void AddOrUpdate(TEntity entity);

        /// <summary>
        /// Deletes the specified entity by id.
        /// </summary>
        /// <param name="entity">The entity id.</param>
        void Delete(TEntity entity);

        void Delete(Expression<Func<TEntity, bool>> filter);

        /// <summary>
        /// Finds the entity by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Found entity</returns>
        TEntity FindById(object id);

        /// <summary>
        /// Get the entity by filter expression
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="includes"></param>
        /// <returns>Entity</returns>
        TEntity Get(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Get the read only entity. (Not tracked)
        /// </summary>
        /// <returns>Entity</returns>
        TEntity GetReadOnly(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Gets the specified entities filtered by specified filter expression and
        /// loaded with specified dependent objects.
        /// </summary>
        /// <param name="filter">The filter to be used in Where clause.</param>
        /// <param name="includes">The related objects to be loaded.</param>
        /// <returns>Materialized result set.</returns>
        List<TEntity> GetMany(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includes);
        
        List<TEntity> GetAll(params Expression<Func<TEntity, object>>[] includes);

        List<TEntity> GetAllReadOnly(params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Gets the read only records only. (Not tracked)
        /// </summary>
        /// <returns>List of read-only records</returns>
        IQueryable<TEntity> GetAllReadOnly();

        /// <summary>
        /// Gets count of the records.
        /// </summary>
        /// <returns>Count of records</returns>
        int Count();

        /// <summary>
        /// Gets count of the records.
        /// </summary>
        /// <returns>Count of records</returns>
        int Count(Expression<Func<TEntity, bool>> filter);
        
        IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> filter);

        IQueryable<TEntity> QueryAll();

        IQueryable<TEntity> GetRangeByIndex<TKey>(int from, int count, Func<TEntity, TKey> order);
    }
}
