using System.Linq.Expressions;

using PCSetupHub.Data.Models.Hardware;
using PCSetupHub.Data.Repositories.Base;

namespace PCSetupHub.Data.Repositories.Interfaces.PcConfigurations
{
	public interface IHardwareComponentRepository<TComponent> : IRepository<TComponent>
		where TComponent : HardwareComponent, new()
	{
		new Task<TComponent?> GetOneAsync(int id);
		new Task<List<TComponent>> GetSomeAsync(Expression<Func<TComponent, bool>> where);
	}
}