using MaginusLunch.Core.Entities;
//using MongoDB.Bson.Serialization;
//using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.MongoDB
{
    public abstract class MongoRepository<T> : IMongoRepository<T>
            where T : Entity
    {
        #region MongoSpecific

        /// <summary>
        /// where you need to define a connectionString with the name of repository
        /// </summary>
        /// <param name="config">config interface to read default settings</param>
        //public MongoRepository(IConfiguration config)
        //{
        //    Collection = Database<T>.GetCollection(config);
        //}

        /// <summary>
        /// with custom settings
        /// </summary>
        /// <param name="connectionString">connection string</param>
        /// <param name="collectionName">collection name</param>
        //public MongoRepository(string connectionString, string collectionName)
        //{
        //    Collection = Database<T>.GetCollectionFromConnectionString(connectionString, collectionName);
        //}

        /// <summary>
        /// its all about the connectionString!
        /// </summary>
        /// <param name="connectionString">in MongoDB connection string form</param>
        /// <param name="asyncRetryPolicy">A Polly Policy for use with the async Repository Operations</param>
        /// <param name="retryPolicy">A Polly Policy for use with the synchronous Repository Operations</param>
        protected MongoRepository(MongoRepositorySettings settings, 
            RetryPolicy asyncRetryPolicy, 
            RetryPolicy retryPolicy)
        {
            //const int maxRetries = 3;
            //AsyncRetryPolicy = RetryPolicy
            //    .Handle<MongoConnectionException>(i =>
            //        i.InnerException.GetType() == typeof(IOException)
            //        || i.InnerException.GetType() == typeof(SocketException))
            //    .RetryAsync(maxRetries);
            //RetryPolicy = RetryPolicy
            //    .Handle<MongoConnectionException>(i =>
            //        i.InnerException.GetType() == typeof(IOException)
            //        || i.InnerException.GetType() == typeof(SocketException))
            //    .Retry(maxRetries, (exception, retryCount, context) =>
            //    {
            //        if (retryCount == maxRetries)
            //        {
            //            _logger.Error("");
            //        }
            //        else
            //        {
            //            _logger.Warn("");
            //        }
            //    });
            //#region todo
            //// to do: this can't be done here. 
            //// There has to be a separate configuration step that executes
            //// prior to the MongoDB Client being initialzed, ever by whatever
            //// instance of a Repository.
            //if (!BsonClassMap.IsClassMapRegistered(typeof(Entity)))
            //{
            //    BsonClassMap.RegisterClassMap<Entity>(cm =>
            //    {
            //        cm.AutoMap();
            //        cm.MapIdMember(e => e.Id).SetIdGenerator(GuidGenerator.Instance);
            //        cm.SetIsRootClass(true);
            //    });
            //}
            //ConfigureSerialzation(); // calling virtual function from constructors is probably a bad idea!
            //#endregion
            AsyncRetryPolicy = asyncRetryPolicy;
            RetryPolicy = retryPolicy;
            Collection = Database<T>.GetCollectionFromConnectionString(settings.ConnectionString);
        }

        /// <summary>
        /// This probably needs to be re-factored away - see Constructor
        /// </summary>
        //public abstract void ConfigureSerialzation();

        public IMongoCollection<T> Collection { get; private set; }

        public FilterDefinitionBuilder<T> Filter
        {
            get { return Builders<T>.Filter; }
        }

        public ProjectionDefinitionBuilder<T> Project
        {
            get { return Builders<T>.Projection; }
        }

        public UpdateDefinitionBuilder<T> Updater
        {
            get { return Builders<T>.Update; }
        }

        /// <summary>
        /// Policy (by Polly) for use in asynchrnous operations
        /// </summary>
        protected readonly RetryPolicy AsyncRetryPolicy;
        /// <summary>
        /// Policy (by Polly) for use in synchrnous operations
        /// </summary>
        protected readonly RetryPolicy RetryPolicy;

        /// <summary>
        /// a piece of shorthand for Collection.Find(filter)
        /// </summary>
        /// <param name="filter">A filter expression to apply</param>
        /// <returns>A Fluent Find Interface</returns>
        private IFindFluent<T, T> Query(Expression<Func<T, bool>> filter)
        {
            return Collection.Find(filter);
        }

        /// <summary>
        /// Begins a Fluent Find Interface with an empty filter
        /// </summary>
        /// <returns>Fluent Find Interface</returns>
        private IFindFluent<T, T> Query()
        {
            return Collection.Find(Filter.Empty);
        }

        #endregion MongoSpecific

        #region CRUD

        #region Delete

        public virtual bool Delete(T entity)
        {
            return Delete(entity.Id);
        }

        public virtual Task<bool> DeleteAsync(T entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            return DeleteAsync(entity.Id, cancellationToken);
        }

        public virtual bool Delete(Guid id)
        {
            //return RetryPolicy.Execute<bool>(() =>
            //{
            //    return Collection.DeleteOne(i => i.Id == id).IsAcknowledged;
            //});
            // OR which is better?
            return RetryPolicy.Execute(() =>
                Collection.DeleteOne(i => i.Id == id).IsAcknowledged);
        }

        public virtual async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken))
        {
            //var deleteResult = await AsyncRetryPolicy.ExecuteAsync<DeleteResult>(() =>
            //    { return Collection.DeleteOneAsync(i => i.Id == id); })
            // OR which is better?
            var deleteResult = await AsyncRetryPolicy.ExecuteAsync((token) =>
                Collection.DeleteOneAsync(i => i.Id == id, token), cancellationToken, false)
                .ConfigureAwait(false);
            return deleteResult.IsAcknowledged;
        }

        public virtual bool Delete(Expression<Func<T, bool>> filter)
        {
            return RetryPolicy.Execute(() =>
                Collection.DeleteMany(filter).IsAcknowledged);
        }

        public virtual async Task<bool> DeleteAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default(CancellationToken))
        {
            var deleteResult = await AsyncRetryPolicy.ExecuteAsync((token) =>
                Collection.DeleteManyAsync(filter, token), cancellationToken, false)
                .ConfigureAwait(false);
            return deleteResult.IsAcknowledged;
        }

        public virtual bool DeleteAll()
        {
            return RetryPolicy.Execute(() =>
                Collection.DeleteMany(Filter.Empty).IsAcknowledged);
        }

        public virtual async Task<bool> DeleteAllAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var deleteResult = await AsyncRetryPolicy.ExecuteAsync((token) =>
                Collection.DeleteManyAsync(Filter.Empty, token), cancellationToken, false)
                .ConfigureAwait(false);
            return deleteResult.IsAcknowledged;
        }
        #endregion Delete

        #region Find

        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> filter)
        {
            return RetryPolicy.Execute(() =>
                Query(filter).ToEnumerable());
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var findResult = await AsyncRetryPolicy.ExecuteAsync((token) =>
                Collection.Find(filter).ToListAsync(token), cancellationToken, false)
                .ConfigureAwait(false);
            return findResult;
        }

        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> filter, int pageIndex, int size)
        {
            return Find(filter, null, pageIndex, size);
        }

        public virtual Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter, int pageIndex, int size, CancellationToken cancellationToken = default(CancellationToken))
        {
            return FindAsync(filter, null, pageIndex, size, cancellationToken);
        }

        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, int pageIndex, int size)
        {
            return Find(filter, order, pageIndex, size, true);
        }

        public virtual Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, int pageIndex, int size, CancellationToken cancellationToken = default(CancellationToken))
        {
            return FindAsync(filter, order, pageIndex, size, true, cancellationToken);
        }

        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, int pageIndex, int size, bool isDescending)
        {
            return RetryPolicy.Execute(() =>
            {
                var query = Query(filter).Skip(pageIndex * size).Limit(size);
                if (order == null)
                {
                    return query.ToEnumerable();
                }
                return (isDescending ? query.SortByDescending(order) : query.SortBy(order)).ToEnumerable();
            });
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, int pageIndex, int size, bool isDescending, CancellationToken cancellationToken = default(CancellationToken))
        {
            var findResult = await AsyncRetryPolicy.ExecuteAsync((token) =>
            {
                var query = Collection.Find(filter).Skip(pageIndex * size).Limit(size);
                if (order == null)
                {
                    return query.ToListAsync(token);
                }
                if (isDescending)
                {
                    return query.SortByDescending(order).ToListAsync(token);
                }
                return query.SortBy(order).ToListAsync();
            }, cancellationToken, false).ConfigureAwait(false);
            return findResult;
        }

        #endregion Find

        #region FindAll

        public virtual IEnumerable<T> FindAll()
        {
            return RetryPolicy.Execute(() => { return Query().ToEnumerable(); });
        }

        public virtual async Task<IEnumerable<T>> FindAllAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var findResult = await AsyncRetryPolicy.ExecuteAsync((token) =>
                Query().ToListAsync(token), cancellationToken, false
            ).ConfigureAwait(false);
            return findResult;
        }

        public virtual IEnumerable<T> FindAll(int pageIndex, int size)
        {
            return FindAll(null, pageIndex, size);
        }

        public virtual Task<IEnumerable<T>> FindAllAsync(int pageIndex, int size, CancellationToken cancellationToken = default(CancellationToken))
        {
            return FindAllAsync(null, pageIndex, size, cancellationToken);
        }

        public virtual IEnumerable<T> FindAll(Expression<Func<T, object>> order, int pageIndex, int size)
        {
            return FindAll(order, pageIndex, size, true);
        }

        public virtual Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, object>> order, int pageIndex, int size, CancellationToken cancellationToken = default(CancellationToken))
        {
            return FindAllAsync(order, pageIndex, size, true, cancellationToken);
        }

        public virtual IEnumerable<T> FindAll(Expression<Func<T, object>> order, int pageIndex, int size, bool isDescending)
        {
            return RetryPolicy.Execute(() =>
            {
                var query = Query().Skip(pageIndex * size).Limit(size);
                if (order == null)
                {
                    return query.ToEnumerable();
                }
                return (isDescending ? query.SortByDescending(order) : query.SortBy(order)).ToEnumerable();
            });
        }

        public virtual async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, object>> order, int pageIndex, int size, bool isDescending, CancellationToken cancellationToken = default(CancellationToken))
        {
            var findResult = await AsyncRetryPolicy.ExecuteAsync((token) =>
            {
                var query = Query().Skip(pageIndex * size).Limit(size);
                if (order == null)
                {
                    return query.ToListAsync(token);
                }
                if (isDescending)
                {
                    return query.SortByDescending(order).ToListAsync(token);
                }
                return query.SortBy(order).ToListAsync(token);
            }, cancellationToken, false).ConfigureAwait(false);
            return findResult;
        }

        #endregion FindAll

        #region Get

        public virtual T Get(Guid id)
        {
            return RetryPolicy.Execute(() => Find(e => e.Id == id).SingleOrDefault());
        }

        public virtual Task<T> GetAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken))
        {
            return AsyncRetryPolicy.ExecuteAsync((token) =>
                Collection.Find(e => e.Id == id).SingleOrDefaultAsync(token), cancellationToken, false);
        }

        #endregion Get

        #region Insert

        public virtual void Insert(T entity)
        {
            RetryPolicy.Execute(() => Collection.InsertOne(entity));
        }

        public virtual Task InsertAsync(T entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            return AsyncRetryPolicy.ExecuteAsync((token) => Collection.InsertOneAsync(entity, null, token), cancellationToken, false);
        }

        public virtual void Insert(IEnumerable<T> entities)
        {
            RetryPolicy.Execute(() => Collection.InsertMany(entities));
        }

        public virtual Task InsertAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken)) 
            => AsyncRetryPolicy.ExecuteAsync((token) => Collection.InsertManyAsync(entities, null, token), cancellationToken, false);

        #endregion Insert

        #region Replace

        public virtual bool Replace(T entity)
        {
            return RetryPolicy.Execute(() => 
                Collection.ReplaceOne(i => i.Id == entity.Id, entity)
                .IsAcknowledged);
        }

        public virtual async Task<bool> ReplaceAsync(T entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            var replaceResult = await AsyncRetryPolicy.ExecuteAsync((token) =>
                Collection.ReplaceOneAsync(e => e.Id == entity.Id, entity, null, token), cancellationToken, false)
                .ConfigureAwait(false);
            return replaceResult.IsAcknowledged;
        }

        public Task ReplaceAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken))
        {
            var tasks = new List<Task>();
            foreach (T entity in entities)
            {
                tasks.Add(ReplaceAsync(entity, cancellationToken));
            }
            return Task.WhenAll(tasks);
        }

        public void Replace(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                Replace(entity);
            }
        }

        #endregion Replace

        #region Update

        public bool Update<TField>(T entity, Expression<Func<T, TField>> field, TField value)
        {
            return Update(entity, Updater.Set(field, value));
        }

        public Task<bool> UpdateAsync<TField>(T entity, Expression<Func<T, TField>> field, TField value, CancellationToken cancellationToken = default(CancellationToken))
        {
            return UpdateAsync(entity, cancellationToken, Updater.Set(field, value));
        }

        public virtual bool Update(Guid id, params UpdateDefinition<T>[] updates)
        {
            return Update(Filter.Eq(i => i.Id, id), updates);
        }

        public virtual Task<bool> UpdateAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken), params UpdateDefinition<T>[] updates)
        {
            return UpdateAsync(Filter.Eq(i => i.Id, id), cancellationToken, updates);
        }

        public virtual bool Update(T entity, params UpdateDefinition<T>[] updates)
        {
            return Update(entity.Id, updates);
        }

        public virtual Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken = default(CancellationToken), params UpdateDefinition<T>[] updates)
        {
            return UpdateAsync(entity.Id, cancellationToken, updates);
        }

        public bool Update<TField>(FilterDefinition<T> filter, Expression<Func<T, TField>> field, TField value)
        {
            return Update(filter, Updater.Set(field, value));
        }

        public Task<bool> UpdateAsync<TField>(FilterDefinition<T> filter, Expression<Func<T, TField>> field, TField value, CancellationToken cancellationToken = default(CancellationToken))
        {
            return UpdateAsync(filter, cancellationToken, Updater.Set(field, value));
        }

        public bool Update(FilterDefinition<T> filter, params UpdateDefinition<T>[] updates)
        {
            return RetryPolicy.Execute(() =>
            {
                var update = Updater.Combine(updates);
                return Collection.UpdateMany(filter, update).IsAcknowledged;
            });
        }

        public async Task<bool> UpdateAsync(FilterDefinition<T> filter, CancellationToken cancellationToken = default(CancellationToken), params UpdateDefinition<T>[] updates)
        {
            var updateResult = await AsyncRetryPolicy.ExecuteAsync((token) =>
                {
                    var update = Updater.Combine(updates);
                    return Collection.UpdateManyAsync(filter, update, null, token);
                }, cancellationToken, false).ConfigureAwait(false);
            return updateResult.IsAcknowledged;
        }

        public bool Update(Expression<Func<T, bool>> filter, params UpdateDefinition<T>[] updates)
        {
            return RetryPolicy.Execute(() =>
            {
                var update = Updater.Combine(updates);
                return Collection.UpdateMany(filter, update).IsAcknowledged;
            });
        }

        public async Task<bool> UpdateAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default(CancellationToken), params UpdateDefinition<T>[] updates)
        {
            var updateResult = await AsyncRetryPolicy.ExecuteAsync((token) =>
            {
                var update = Updater.Combine(updates);
                return Collection.UpdateManyAsync(filter, update, null, token);
            }, cancellationToken, false).ConfigureAwait(false);
            return updateResult.IsAcknowledged;
        }

        #endregion Update

        #endregion CRUD

        #region Utils

        public bool Any(Expression<Func<T, bool>> filter)
        {
            return RetryPolicy.Execute(() => Collection.AsQueryable().Any(filter));
        }

        #region Count
        public long Count(Expression<Func<T, bool>> filter)
        {
            return RetryPolicy.Execute(() => Collection.Count(filter));
        }

        public Task<long> CountAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default(CancellationToken))
        {
            return AsyncRetryPolicy.ExecuteAsync((token) => Collection.CountAsync(filter, null, token), cancellationToken, false);
        }

        public long Count()
        {
            return RetryPolicy.Execute(() => Collection.Count(Filter.Empty));
        }

        public Task<long> CountAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return AsyncRetryPolicy.ExecuteAsync((token) => Collection.CountAsync(Filter.Empty, null, token), cancellationToken, false);
        }
        #endregion Count

        #endregion Utils
    }
}