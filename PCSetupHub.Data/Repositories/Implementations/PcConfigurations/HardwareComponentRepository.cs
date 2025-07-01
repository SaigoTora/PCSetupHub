using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

using PCSetupHub.Data.Models.Hardware;
using PCSetupHub.Data.Repositories.Base;
using PCSetupHub.Data.Repositories.Interfaces.PcConfigurations;

namespace PCSetupHub.Data.Repositories.Implementations.PcConfigurations
{
	public abstract class HardwareComponentRepository<TComponent>(PcSetupContext context)
		: BaseRepo<TComponent>(context), IHardwareComponentRepository<TComponent>
		where TComponent : HardwareComponent, new()
	{
		protected abstract IQueryable<TComponent> GetQuery();

		public new async Task<TComponent?> GetOneAsync(int id)
			=> await GetQuery().FirstOrDefaultAsync(x => x.Id == id);
		public new async Task<List<TComponent>> GetSomeAsync(Expression<Func<TComponent,
			bool>> where)
		{
			return await GetQuery().Where(where).ToListAsync();
		}
	}
}