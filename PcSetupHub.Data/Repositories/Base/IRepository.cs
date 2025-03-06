using System.Linq.Expressions;

using PCSetupHub.Data.Models.Base;

namespace PCSetupHub.Data.Repositories.Base
{
	public interface IRepository<T> where T : BaseEntity, new()
	{
		Task<T> AddAsync(T entity);
		Task<List<T>> AddAsync(IList<T> entities);

		Task<bool> DeleteAsync(int id);
		Task<bool> DeleteAsync(T entity);

		Task<T?> GetOneAsync(int id);
		Task<List<T>> GetAllAsync();
		Task<List<T>> GetSomeAsync(Expression<Func<T, bool>> where);
		Task<List<T>> GetAllAsync<TSortField>(Expression<Func<T, TSortField>> orderBy,
			bool ascending);

		Task<int> UpdateAsync(T entity);
		Task<int> UpdateAsync(IList<T> entities);

		Task<List<T>> ExecuteQueryAsync(string sql);
		Task<List<T>> ExecuteQueryAsync(string sql, params object[] sqlParametersObjects);
	}
}