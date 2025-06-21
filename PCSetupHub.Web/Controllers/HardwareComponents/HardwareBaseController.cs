using Microsoft.AspNetCore.Mvc;

using PCSetupHub.Core.Extensions;
using PCSetupHub.Data.Models.Hardware;
using PCSetupHub.Data.Repositories.Base;
using PCSetupHub.Data.Repositories.Interfaces.PcConfigurations;
using PCSetupHub.Data.Repositories.Interfaces.Users;
using PCSetupHub.Web.ViewModels;

namespace PCSetupHub.Web.Controllers.HardwareComponents
{
	public abstract class HardwareBaseController<TComponent> : Controller
		where TComponent : HardwareComponent, new()
	{
		protected abstract string ComponentName { get; }
		protected abstract PcConfigurationIncludes PcConfigurationIncludes { get; }

		private readonly IPcConfigurationRepository _pcConfigRepository;
		private readonly IRepository<TComponent> _componentRepository;
		private readonly IUserRepository _userRepository;

		protected HardwareBaseController(IPcConfigurationRepository pcConfigRepository,
			IRepository<TComponent> componentRepository, IUserRepository userRepository)
		{
			_pcConfigRepository = pcConfigRepository;
			_componentRepository = componentRepository;
			_userRepository = userRepository;
		}

		#region Abstract methods
		protected abstract TComponent? GetComponent(PcConfiguration pcConfiguration);
		protected abstract void ChangeComponent(PcConfiguration pcConfiguration,
			TComponent component);
		protected abstract void ClearComponent(PcConfiguration pcConfiguration);
		#endregion

		#region Action methods
		[HttpGet("Search/{pcConfigurationId}")]
		public async Task<IActionResult> SearchAsync(int pcConfigurationId, int page = 1,
			string? searchQuery = null)
		{
			if (!await HasAccessToPcConfigurationAsync(pcConfigurationId))
				return StatusCode(403);

			PcConfiguration? pcConfig = await _pcConfigRepository.GetByIdAsync(pcConfigurationId,
				PcConfigurationIncludes);
			if (pcConfig == null)
				return NotFound();

			var filteredComponents = await GetFilteredComponentsAsync(searchQuery);
			int totalItems = filteredComponents.Count;

			SearchComponentViewModel model = new(pcConfigurationId, searchQuery,
				GetComponent(pcConfig)?.Id, ComponentName, page, totalItems);

			model.Components = GetPagedItems(filteredComponents, model.Page, model.PageSize);
			return View(model);
		}

		[HttpPost("Select/{pcConfigurationId}/{componentId}")]
		public async Task<IActionResult> SelectAsync(int pcConfigurationId, int componentId)
		{
			if (!await HasAccessToPcConfigurationAsync(pcConfigurationId))
				return StatusCode(403);

			PcConfiguration? pcConfig = await _pcConfigRepository.GetByIdAsync(pcConfigurationId,
				PcConfigurationIncludes);
			if (pcConfig == null)
				return NotFound();

			TComponent? component = await _componentRepository.GetOneAsync(componentId);

			if (component == null)
				return NotFound();
			if (!component.IsDefault)
				return StatusCode(403);

			await TryDeleteCurrentAsync(pcConfig);

			ChangeComponent(pcConfig, component);
			await _pcConfigRepository.UpdateAsync(pcConfig);

			return RedirectToPcSetup(pcConfigurationId);
		}

		[HttpPost("Clear/{pcConfigurationId}")]
		public async Task<IActionResult> ClearAsync(int pcConfigurationId)
		{
			if (!await HasAccessToPcConfigurationAsync(pcConfigurationId))
				return StatusCode(403);

			PcConfiguration? pcConfig = await _pcConfigRepository.GetByIdAsync(pcConfigurationId,
				PcConfigurationIncludes);
			if (pcConfig == null)
				return NotFound();

			TComponent? component = GetComponent(pcConfig);
			if (component != null && !component.IsDefault)
				return StatusCode(403);

			ClearComponent(pcConfig);
			await _pcConfigRepository.UpdateAsync(pcConfig);

			return RedirectToPcSetup(pcConfigurationId);
		}

		[HttpGet("Create/{pcConfigurationId}")]
		public async Task<IActionResult> CreateAsync(int pcConfigurationId)
		{
			if (!await HasAccessToPcConfigurationAsync(pcConfigurationId))
				return StatusCode(403);

			return View(new TComponent());
		}
		
		[HttpPost("Create/{pcConfigurationId}")]
		public async Task<IActionResult> CreateAsync(int pcConfigurationId, TComponent component)
		{
			if (component == null)
				return NotFound();
			component.IsDefault = false;

			if (!ModelState.IsValid)
			{
				SetFirstError();
				return View(component);
			}

			if (!await HasAccessToPcConfigurationAsync(pcConfigurationId))
				return StatusCode(403);

			PcConfiguration? pcConfig = await _pcConfigRepository.GetByIdAsync(pcConfigurationId,
				PcConfigurationIncludes);
			if (pcConfig == null)
				return NotFound();

			await TryDeleteCurrentAsync(pcConfig);

			await _componentRepository.AddAsync(component);
			ChangeComponent(pcConfig, component);
			await _pcConfigRepository.UpdateAsync(pcConfig);

			return RedirectToPcSetup(pcConfigurationId);
		}
		#endregion

		#region Authorization and helpers
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
		protected async Task<bool> TryDeleteCurrentAsync(PcConfiguration pcConfig)
		{
			// Delete the current component if it is not default.
			TComponent? currentComponent = GetComponent(pcConfig);
			if (currentComponent != null && !currentComponent.IsDefault)
			{
				await _componentRepository.DeleteAsync(currentComponent.Id);
				return true;
			}

			return false;
		}

		private void SetFirstError()
		{
			ViewData["FirstError"] = ModelState.Values
				.SelectMany(v => v.Errors)
				.Select(e => e.ErrorMessage)
				.FirstOrDefault();
		}
		#endregion
	}
}