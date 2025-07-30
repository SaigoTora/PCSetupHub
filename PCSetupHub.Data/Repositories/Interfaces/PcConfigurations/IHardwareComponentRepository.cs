using System.Linq.Expressions;

using PCSetupHub.Data.Models.Hardware;
using PCSetupHub.Data.Repositories.Base;

namespace PCSetupHub.Data.Repositories.Interfaces.PcConfigurations
{
	public interface IHardwareComponentRepository<TComponent> : IRepository<TComponent>
		where TComponent : HardwareComponent, new()
	{
		/// <summary>
		/// Retrieves a hardware component by its ID using the extended query.
		/// </summary>
		/// <param name="id">The ID of the component to retrieve.</param>
		/// <returns>The matching hardware component if found; otherwise, null.</returns>
		new Task<TComponent?> GetOneAsync(int id);

		/// <summary>
		/// Retrieves hardware components matching the given condition using the extended query.
		/// </summary>
		/// <param name="where">A predicate to filter components.</param>
		/// <returns>A list of matching hardware components.</returns>
		new Task<List<TComponent>> GetSomeAsync(Expression<Func<TComponent, bool>> where);
	}
}