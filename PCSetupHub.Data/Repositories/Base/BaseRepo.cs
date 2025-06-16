using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

using PCSetupHub.Data.Models.Base;

namespace PCSetupHub.Data.Repositories.Base
{
	public class BaseRepo<T> : IDisposable, IRepository<T> where T : BaseEntity, new()
	{
		protected PcSetupContext Context => _context;

		private readonly DbSet<T> _table;
		private readonly PcSetupContext _context;
		private bool _disposed = false;

		public BaseRepo(PcSetupContext context)
		{
			_context = context;
			_table = _context.Set<T>();
		}

		public async Task<T> AddAsync(T entity)
		{
			await _table.AddAsync(entity);
			await _context.SaveChangesAsync();
			return entity;
		}
		public async Task<List<T>> AddAsync(IList<T> entities)
		{
			await _table.AddRangeAsync(entities);
			await _context.SaveChangesAsync();
			return [.. entities];
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var entity = await _table.FindAsync(id);
			if (entity == null) return false;

			_table.Remove(entity);
			await _context.SaveChangesAsync();
			return true;
		}
		public async Task<bool> DeleteAsync(T entity)
		{
			ArgumentNullException.ThrowIfNull(entity);
			_table.Remove(entity);
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<T?> GetOneAsync(int id) => await _table.FindAsync(id);
		public async Task<List<T>> GetAllAsync() => await _table.ToListAsync();
		public async Task<List<T>> GetAllAsync<TSortField>(Expression<Func<T, TSortField>> orderBy,
			bool ascending)
		{
			return await (ascending ? _table.OrderBy(orderBy) : _table.OrderByDescending(orderBy))
				.ToListAsync();
		}
		public async Task<List<T>> GetSomeAsync(Expression<Func<T, bool>> where)
			=> await _table.Where(where).ToListAsync();
		public async Task<List<T>> GetPageAsync(int pageNumber, int pageSize)
		{
			return await _table
				   .Skip((pageNumber - 1) * pageSize)
				   .Take(pageSize)
				   .ToListAsync();
		}
		public async Task<List<T>> GetPageAsync(Expression<Func<T, bool>> where, int pageNumber,
			int pageSize)
		{
			return await _table
				.Where(where)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();
		}

		public async Task<int> CountAsync() => await _table.CountAsync();
		public async Task<int> CountAsync(Expression<Func<T, bool>> where)
		{
			if (where == null)
				return await _table.CountAsync();

			return await _table.CountAsync(where);
		}

		public async Task<int> UpdateAsync(T entity)
		{
			_table.Update(entity);
			return await _context.SaveChangesAsync();
		}
		public async Task<int> UpdateAsync(IList<T> entities)
		{
			_table.UpdateRange(entities);
			return await _context.SaveChangesAsync();
		}

		public async Task<List<T>> ExecuteQueryAsync(string sql)
			=> await _table.FromSqlRaw(sql).ToListAsync();
		public async Task<List<T>> ExecuteQueryAsync(string sql,
			params object[] sqlParametersObjects)
			=> await _table.FromSqlRaw(sql, sqlParametersObjects).ToListAsync();

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
					_context?.Dispose();

				_disposed = true;
			}
		}
	}
}