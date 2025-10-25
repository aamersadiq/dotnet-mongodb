using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankAccountManagement.Core.Interfaces.Repositories
{
    /// <summary>
    /// Generic repository interface for CRUD operations
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Gets all entities
        /// </summary>
        /// <returns>Collection of all entities</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Gets an entity by its identifier
        /// </summary>
        /// <param name="id">Entity identifier</param>
        /// <returns>Entity if found, null otherwise</returns>
        Task<T> GetByIdAsync(string id);

        /// <summary>
        /// Adds a new entity
        /// </summary>
        /// <param name="entity">Entity to add</param>
        /// <returns>Added entity</returns>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Updates an existing entity
        /// </summary>
        /// <param name="entity">Entity to update</param>
        /// <returns>True if update was successful, false otherwise</returns>
        Task<bool> UpdateAsync(T entity);

        /// <summary>
        /// Deletes an entity by its identifier
        /// </summary>
        /// <param name="id">Entity identifier</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        Task<bool> DeleteAsync(string id);
    }
}