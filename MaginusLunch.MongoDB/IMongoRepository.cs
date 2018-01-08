using MaginusLunch.Core.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.MongoDB
{
    /// <summary>
    /// MongoDB Repository interface
    /// </summary>
    /// <typeparam name="T">The IEntity Class represented</typeparam>
    public interface IMongoRepository<T> where T : IEntity
    {
        #region MongoSpecific

        /// <summary>
        /// MongoDB Collection accessor
        /// </summary>
        IMongoCollection<T> Collection { get; }

        /// <summary>
        /// filter for collection
        /// </summary>
        FilterDefinitionBuilder<T> Filter { get; }

        /// <summary>
        /// projector for collection
        /// </summary>
        ProjectionDefinitionBuilder<T> Project { get; }

        /// <summary>
        /// updater for collection
        /// </summary>
        UpdateDefinitionBuilder<T> Updater { get; }

        #endregion MongoSpecific

        #region CRUD

        #region Delete

        /// <summary>
        /// delete by id
        /// </summary>
        /// <param name="id">id</param>
        bool Delete(Guid id);

        /// <summary>
        /// delete entity
        /// </summary>
        /// <param name="entity">entity</param>
        bool Delete(T entity);

        /// <summary>
        /// delete items with filter
        /// </summary>
        /// <param name="filter">expression filter</param>
        bool Delete(Expression<Func<T, bool>> filter);

        /// <summary>
        /// delete all documents
        /// </summary>
        bool DeleteAll();

        /// <summary>
        /// delete by id
        /// </summary>
        /// <param name="id">id</param>
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// delete entity
        /// </summary>
        /// <param name="entity">entity</param>
        Task<bool> DeleteAsync(T entity, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// delete items with filter
        /// </summary>
        /// <param name="filter">expression filter</param>
        Task<bool> DeleteAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// delete all documents
        /// </summary>
        Task<bool> DeleteAllAsync(CancellationToken cancellationToken = default(CancellationToken));

        #endregion Delete

        #region Find

        /// <summary>
        /// find entities
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>collection of entity</returns>
        IEnumerable<T> Find(Expression<Func<T, bool>> filter);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter, 
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// find entities with paging
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <returns>collection of entity</returns>
        IEnumerable<T> Find(Expression<Func<T, bool>> filter, int pageIndex, int size);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter, int pageIndex, int size, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// find entities with paging and ordering
        /// default ordering is descending
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <returns>collection of entity</returns>
        IEnumerable<T> Find(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, int pageIndex, int size);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, int pageIndex, int size, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// find entities with paging and ordering in direction
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <param name="isDescending">ordering direction</param>
        /// <returns>collection of entity</returns>
        IEnumerable<T> Find(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, int pageIndex, int size, bool isDescending);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, int pageIndex, int size, bool isDescending, CancellationToken cancellationToken = default(CancellationToken));

        #endregion Find

        #region FindAll

        /// <summary>
        /// fetch all items in collection
        /// </summary>
        /// <returns>collection of entity</returns>
        IEnumerable<T> FindAll();
        Task<IEnumerable<T>> FindAllAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// fetch all items in collection with paging
        /// </summary>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <returns>collection of entity</returns>
        IEnumerable<T> FindAll(int pageIndex, int size);
        Task<IEnumerable<T>> FindAllAsync(int pageIndex, int size, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// fetch all items in collection with paging and ordering
        /// default ordering is descending
        /// </summary>
        /// <param name="order">ordering parameters</param>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <returns>collection of entity</returns>
        IEnumerable<T> FindAll(Expression<Func<T, object>> order, int pageIndex, int size);
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, object>> order, int pageIndex, int size, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// fetch all items in collection with paging and ordering in direction
        /// </summary>
        /// <param name="order">ordering parameters</param>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <param name="isDescending">ordering direction</param>
        /// <returns>collection of entity</returns>
        IEnumerable<T> FindAll(Expression<Func<T, object>> order, int pageIndex, int size, bool isDescending);
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, object>> order, int pageIndex, int size, bool isDescending, CancellationToken cancellationToken = default(CancellationToken));

        #endregion FindAll

        #region Get

        /// <summary>
        /// get by id
        /// </summary>
        /// <param name="id">id value</param>
        /// <returns>entity of <typeparamref name="T"/></returns>
        T Get(Guid id);
        Task<T> GetAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));

        #endregion Get

        #region Insert

        /// <summary>
        /// insert entity
        /// </summary>
        /// <param name="entity">entity</param>
        void Insert(T entity);

        /// <summary>
        /// insert entity
        /// </summary>
        /// <param name="entity">entity</param>
        Task InsertAsync(T entity, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// insert entity collection
        /// </summary>
        /// <param name="entities">collection of entities</param>
        void Insert(IEnumerable<T> entities);

        /// <summary>
        /// insert entity collection
        /// </summary>
        /// <param name="entities">collection of entities</param>
        Task InsertAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken));

        #endregion Insert

        #region Replace

        /// <summary>
        /// replace an existing entity
        /// </summary>
        /// <param name="entity">entity</param>
        bool Replace(T entity);

        /// <summary>
        /// replace an existing entity
        /// </summary>
        /// <param name="entity">entity</param>
        Task<bool> ReplaceAsync(T entity, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// replace collection of entities
        /// </summary>
        /// <param name="entities">collection of entities</param>
        void Replace(IEnumerable<T> entities);
        Task ReplaceAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken));

        #endregion Replace

        #region Update

        /// <summary>
        /// update an entity with updated fields
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="update">updated field(s)</param>
        /// <returns>true if successful, otherwise false</returns>
        bool Update(Guid id, params UpdateDefinition<T>[] updates);

        /// <summary>
        /// update an entity with updated fields
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="update">updated field(s)</param>
        /// <returns>true if successful, otherwise false</returns>
        bool Update(T entity, params UpdateDefinition<T>[] updates);

        /// <summary>
        /// update found entities by filter with updated fields
        /// </summary>
        /// <param name="filter">collection filter</param>
        /// <param name="update">updated field(s)</param>
        /// <returns>true if successful, otherwise false</returns>
        bool Update(FilterDefinition<T> filter, params UpdateDefinition<T>[] updates);

        /// <summary>
        /// update found entities by filter with updated fields
        /// </summary>
        /// <param name="filter">collection filter</param>
        /// <param name="update">updated field(s)</param>
        /// <returns>true if successful, otherwise false</returns>
        bool Update(Expression<Func<T, bool>> filter, params UpdateDefinition<T>[] updates);

        /// <summary>
        /// update a property field in an entity
        /// </summary>
        /// <typeparam name="TField">field type</typeparam>
        /// <param name="entity">entity</param>
        /// <param name="field">field</param>
        /// <param name="value">new value</param>
        /// <returns>true if successful, otherwise false</returns>
        bool Update<TField>(T entity, Expression<Func<T, TField>> field, TField value);

        /// <summary>
        /// update a property field in entities
        /// </summary>
        /// <typeparam name="TField">field type</typeparam>
        /// <param name="filter">filter</param>
        /// <param name="field">field</param>
        /// <param name="value">new value</param>
        /// <returns>true if successful, otherwise false</returns>
        bool Update<TField>(FilterDefinition<T> filter, Expression<Func<T, TField>> field, TField value);

        /// <summary>
        /// update a property field in an entity
        /// </summary>
        /// <typeparam name="TField">field type</typeparam>
        /// <param name="entity">entity</param>
        /// <param name="field">field</param>
        /// <param name="value">new value</param>
        Task<bool> UpdateAsync<TField>(T entity, Expression<Func<T, TField>> field, TField value, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// update an entity with updated fields
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="updates">updated field(s)</param>
        Task<bool> UpdateAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken), params UpdateDefinition<T>[] updates);

        /// <summary>
        /// update an entity with updated fields
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="updates">updated field(s)</param>
        Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken = default(CancellationToken), params UpdateDefinition<T>[] updates);

        /// <summary>
        /// update a property field in entities
        /// </summary>
        /// <typeparam name="TField">field type</typeparam>
        /// <param name="filter">filter</param>
        /// <param name="field">field</param>
        /// <param name="value">new value</param>
        Task<bool> UpdateAsync<TField>(FilterDefinition<T> filter, Expression<Func<T, TField>> field, TField value, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// update found entities by filter with updated fields
        /// </summary>
        /// <param name="filter">collection filter</param>
        /// <param name="updates">updated field(s)</param>
        Task<bool> UpdateAsync(FilterDefinition<T> filter, CancellationToken cancellationToken = default(CancellationToken), params UpdateDefinition<T>[] updates);

        /// <summary>
        /// update found entities by filter with updated fields
        /// </summary>
        /// <param name="filter">collection filter</param>
        /// <param name="updates">updated field(s)</param>
        Task<bool> UpdateAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default(CancellationToken), params UpdateDefinition<T>[] updates);
        #endregion Update

        #endregion CRUD

        #region Utils
        /// <summary>
        /// validate if filter result exists
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>true if exists, otherwise false</returns>
        bool Any(Expression<Func<T, bool>> filter);

        #region Count
        /// <summary>
        /// get number of filtered documents
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>number of documents</returns>
        long Count(Expression<Func<T, bool>> filter);

        /// <summary>
        /// get number of filtered documents
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>number of documents</returns>
        Task<long> CountAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// get number of documents in collection
        /// </summary>
        /// <returns>number of documents</returns>
        long Count();

        /// <summary>
        /// get number of documents in collection
        /// </summary>
        /// <returns>number of documents</returns>
        Task<long> CountAsync(CancellationToken cancellationToken = default(CancellationToken));
        #endregion Count

        #endregion Utils
    }
}

