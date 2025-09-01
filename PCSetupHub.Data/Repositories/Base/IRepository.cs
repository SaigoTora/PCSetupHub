using System.Linq.Expressions;

using PCSetupHub.Data.Models.Base;

namespace PCSetupHub.Data.Repositories.Base
{
	public interface IRepository<T> where T : BaseEntity, new()
	{
		#region Create
		/// <summary>
		/// Adds a new entity to the database.
		/// </summary>
		/// <param name="entity">The entity to add.</param>
		/// <returns>The added entity.</returns>
		Task<T> AddAsync(T entity);

		/// <summary>
		/// Adds multiple entities to the database.
		/// </summary>
		/// <param name="entities">The list of entities to add.</param>
		/// <returns>The added entities.</returns>
		Task<List<T>> AddAsync(IList<T> entities);
		#endregion

		#region Delete
		/// <summary>
		/// Deletes an entity by its ID.
		/// </summary>
		/// <param name="id">The ID of the entity to delete.</param>
		/// <returns>True if the entity was deleted; otherwise, false.</returns>
		Task<bool> DeleteAsync(int id);

		/// <summary>
		/// Deletes the specified entity from the database.
		/// </summary>
		/// <param name="entity">The entity to delete.</param>
		/// <returns>True if the entity was deleted; otherwise, false.</returns>
		Task<bool> DeleteAsync(T entity);
		#endregion

		#region Read
		/// <summary>
		/// Retrieves an entity by its ID.
		/// </summary>
		/// <param name="id">The ID of the entity to retrieve.</param>
		/// <returns>The entity if found; otherwise, null.</returns>
		Task<T?> GetOneAsync(int id);

		/// <summary>
		/// Retrieves all entities from the database.
		/// </summary>
		/// <returns>A list of all entities.</returns>
		Task<List<T>> GetAllAsync();

		/// <summary>
		/// Retrieves all entities sorted by the specified field.
		/// </summary>
		/// <typeparam name="TSortField">The field type to sort by.</typeparam>
		/// <param name="orderBy">The sorting expression.</param>
		/// <param name="ascending">Whether to sort ascending or descending.</param>
		/// <returns>A sorted list of entities.</returns>
		Task<List<T>> GetAllAsync<TSortField>(Expression<Func<T, TSortField>> orderBy,
			bool ascending);

		/// <summary>
		/// Retrieves all entities that match the specified condition.
		/// </summary>
		/// <param name="where">The filtering condition.</param>
		/// <returns>A list of matching entities.</returns>
		Task<List<T>> GetSomeAsync(Expression<Func<T, bool>> where);

		/// <summary>
		/// Retrieves entities that match the specified condition and projects them into a different type.
		/// </summary>
		/// <typeparam name="TResult">The type of the projected result.</typeparam>
		/// <param name="where">The filtering condition.</param>
		/// <param name="select">The projection expression specifying which fields or type to return.</param>
		/// <returns>An array of projected results.</returns>
		Task<TResult[]> GetSomeAsync<TResult>(Expression<Func<T, bool>> where,
			Expression<Func<T, TResult>> select);

		/// <summary>
		/// Retrieves a specific page of entities sorted by ID.
		/// </summary>
		/// <param name="pageNumber">The page number (1-based).</param>
		/// <param name="pageSize">The number of items per page.</param>
		/// <returns>A list of entities for the specified page.</returns>
		Task<List<T>> GetPageAsync(int pageNumber, int pageSize);

		/// <summary>
		/// Retrieves a specific page of entities that match the condition and are sorted by ID.
		/// </summary>
		/// <param name="where">The filtering condition.</param>
		/// <param name="pageNumber">The page number (1-based).</param>
		/// <param name="pageSize">The number of items per page.</param>
		/// <returns>A list of matching entities for the specified page.</returns>
		Task<List<T>> GetPageAsync(Expression<Func<T, bool>> where, int pageNumber, int pageSize);


		/// <summary>
		/// Counts the total number of entities in the database.
		/// </summary>
		/// <returns>The total number of entities.</returns>
		Task<int> CountAsync();

		/// <summary>
		/// Counts the number of entities that match the specified condition.
		/// </summary>
		/// <param name="where">The filtering condition.</param>
		/// <returns>The number of matching entities.</returns>
		Task<int> CountAsync(Expression<Func<T, bool>> where);
		#endregion

		#region Update
		/// <summary>
		/// Updates an existing entity in the database.
		/// </summary>
		/// <param name="entity">The entity to update.</param>
		/// <returns>The number of affected rows.</returns>
		Task<int> UpdateAsync(T entity);

		/// <summary>
		/// Updates multiple entities in the database.
		/// </summary>
		/// <param name="entities">The list of entities to update.</param>
		/// <returns>The number of affected rows.</returns>
		Task<int> UpdateAsync(IList<T> entities);
		#endregion

		/// <summary>
		/// Executes a raw SQL query and returns the result as a list of entities.
		/// </summary>
		/// <param name="sql">The SQL query to execute.</param>
		/// <returns>A list of resulting entities.</returns>
		Task<List<T>> ExecuteQueryAsync(string sql);

		/// <summary>
		/// Executes a raw SQL query with parameters and returns the result as a list of entities.
		/// </summary>
		/// <param name="sql">The SQL query to execute.</param>
		/// <param name="sqlParametersObjects">The SQL parameters.</param>
		/// <returns>A list of resulting entities.</returns>
		Task<List<T>> ExecuteQueryAsync(string sql, params object[] sqlParametersObjects);
	}
}