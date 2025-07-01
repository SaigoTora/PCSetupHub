using Microsoft.AspNetCore.Mvc;

using PCSetupHub.Core.Extensions;
using PCSetupHub.Data.Models.Attributes;
using PCSetupHub.Data.Models.Hardware;
using PCSetupHub.Data.Repositories.Base;
using PCSetupHub.Data.Repositories.Interfaces.Users;

namespace PCSetupHub.Web.Controllers.HardwareComponents
{
	public abstract class HardwareBaseController<TComponent> : Controller
		where TComponent : HardwareComponent, new()
	{
		protected abstract string ComponentName { get; }
		protected abstract bool IsComponentColorful { get; }

		protected readonly IUserRepository UserRepository;
		protected readonly IRepository<TComponent> ComponentRepository;
		protected readonly IRepository<Color> ColorRepository;

		protected HardwareBaseController(IRepository<TComponent> componentRepository,
			IRepository<Color> colorRepository, IUserRepository userRepository)
		{
			ComponentRepository = componentRepository;
			ColorRepository = colorRepository;
			UserRepository = userRepository;
		}

		#region Authorization and helpers
		protected async Task<bool> HasAccessToPcConfigurationAsync(int pcConfigurationId)
		{
			int? userId = User.GetId();
			if (!userId.HasValue || !await UserRepository.UserHasPcConfigurationAsync(userId.Value,
				pcConfigurationId))
				return false;

			return true;
		}
		protected string? GetSearchQuery()
			=> Request.Query[$"{ComponentName}SearchQuery"].ToString();
		protected IActionResult RedirectToPcSetup(int pcConfigurationId)
			=> RedirectToAction("Index", "PcSetup", new { id = pcConfigurationId });

		protected async Task<List<HardwareComponent>> GetFilteredComponentsAsync(
			string? searchQuery)
		{
			var defaultComponents = await ComponentRepository.GetSomeAsync(p => p.IsDefault);

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

		protected async Task RemoveInvalidColorIdsAsync(List<int> colorIds)
		{
			if (colorIds == null || colorIds.Count == 0)
				return;

			var existingColorIds = (await ColorRepository.GetAllAsync(c => c.Id, true))
				.Select(c => c.Id)
				.ToHashSet();

			colorIds.RemoveAll(id => !existingColorIds.Contains(id));
		}

		protected void SetFirstError()
		{
			ViewData["FirstError"] = ModelState.Values
				.SelectMany(v => v.Errors)
				.Select(e => e.ErrorMessage)
				.FirstOrDefault();
		}
		protected async Task SetColorsAsync(List<int>? selectedColorsId = null)
		{
			if (IsComponentColorful)
			{
				ViewData["Colors"] = await ColorRepository.GetAllAsync(c => c.Id, true);
				if (selectedColorsId != null)
					ViewData["SelectedColorsId"] = selectedColorsId;
			}
		}
		#endregion
	}
}