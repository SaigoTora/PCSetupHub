using Microsoft.AspNetCore.Mvc;

using PCSetupHub.Core.Extensions;
using PCSetupHub.Data.Models.Hardware;
using PCSetupHub.Data.Repositories.Base;
using PCSetupHub.Data.Repositories.Interfaces.Users;

namespace PCSetupHub.Web.Controllers.HardwareComponents
{
	public abstract class HardwareBaseController<TComponent> : Controller
		where TComponent : HardwareComponent, new()
	{
		private readonly IRepository<TComponent> _componentRepository;
		private readonly IUserRepository _userRepository;

		protected HardwareBaseController(IRepository<TComponent> componentRepository,
			IUserRepository userRepository)
		{
			_componentRepository = componentRepository;
			_userRepository = userRepository;
		}

		protected async Task<bool> HasAccessToPcConfigurationAsync(int pcConfigurationId)
		{
			int? userId = User.GetId();
			if (!userId.HasValue || !await _userRepository.UserHasPcConfigurationAsync(userId.Value,
				pcConfigurationId))
				return false;

			return true;
		}
		protected IActionResult RedirectToPcSetup(int pcConfigurationId)
			=> RedirectToAction("Index", "PcSetup", new { id = pcConfigurationId });
		protected async Task<List<HardwareComponent>> GetFilteredComponentsAsync(
			string? searchQuery)
		{
			var defaultComponents = await _componentRepository.GetSomeAsync(p => p.IsDefault);

			// Filter in memory because DisplayName is a computed property,
			// not mapped in the database
			return [.. defaultComponents
				.Where(c => string.IsNullOrEmpty(searchQuery)
				|| c.DisplayName.Contains(searchQuery, StringComparison.CurrentCultureIgnoreCase))
				.OrderBy(c => c.DisplayName)];
		}
		protected List<HardwareComponent> GetPagedItems(List<HardwareComponent> components,
			int page, int pageSize)
		{
			return [.. components
				.Skip((page - 1) * pageSize)
				.Take(pageSize)];
		}
	}
}