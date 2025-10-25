using BankAccountManagement.Core.Interfaces.Repositories;
using MongoDB.Driver;

namespace BankAccountManagement.Infrastructure.Repositories
{
    /// <summary>
    /// Base implementation of the generic repository interface
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public abstract class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly IMongoCollection<T> _collection;

        /// <summary>
        /// Initializes a new instance of the BaseRepository class
        /// </summary>
        /// <param name="collection">MongoDB collection</param>
        public BaseRepository(IMongoCollection<T> collection)
        {
            _collection = collection ?? throw new ArgumentNullException(nameof(collection));
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        /// <inheritdoc/>
        public virtual async Task<T> GetByIdAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq("Id", id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        /// <inheritdoc/>
        public virtual async Task<T> AddAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        /// <inheritdoc/>
        public virtual async Task<bool> UpdateAsync(T entity)
        {
            var filter = Builders<T>.Filter.Eq("Id", GetId(entity));
            var result = await _collection.ReplaceOneAsync(filter, entity);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        /// <inheritdoc/>
        public virtual async Task<bool> DeleteAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq("Id", id);
            var result = await _collection.DeleteOneAsync(filter);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        /// <summary>
        /// Gets the ID value from an entity
        /// </summary>
        /// <param name="entity">Entity instance</param>
        /// <returns>ID value as string</returns>
        protected abstract string GetId(T entity);
    }
}