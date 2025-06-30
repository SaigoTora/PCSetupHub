using Microsoft.AspNetCore.Mvc;

using PCSetupHub.Data.Models.Attributes;
using PCSetupHub.Data.Models.Hardware;
using PCSetupHub.Data.Repositories.Base;
using PCSetupHub.Data.Repositories.Interfaces.PcConfigurations;
using PCSetupHub.Data.Repositories.Interfaces.Users;
using PCSetupHub.Web.ViewModels;

namespace PCSetupHub.Web.Controllers.HardwareComponents
{
	public abstract class HardwareSingleController<TComponent> : HardwareBaseController<TComponent>
		where TComponent : HardwareComponent, new()
	{
		protected abstract PcConfigurationIncludes PcConfigurationIncludes { get; }

		private readonly IPcConfigurationRepository _pcConfigRepository;

		protected HardwareSingleController(IPcConfigurationRepository pcConfigRepository,
			IRepository<TComponent> componentRepository, IRepository<Color> colorRepository,
			IUserRepository userRepository)
			: base(componentRepository, colorRepository, userRepository)
		{
			_pcConfigRepository = pcConfigRepository;
		}

		#region Abstract methods
		protected abstract TComponent? GetComponent(PcConfiguration pcConfiguration);
		protected abstract void ChangeComponent(PcConfiguration pcConfiguration,
			TComponent component);
		protected abstract void ClearComponent(PcConfiguration pcConfiguration);
		protected abstract bool IsComponentInPcConfig(PcConfiguration pcConfig, int componentId);
		#endregion

		#region Virtual methods
		protected virtual Task<TComponent> UpdateColorRelationshipsAsync(TComponent component,
			List<int> colorIds)
			=> Task.FromResult(component);
		#endregion

		#region Action methods
		[HttpGet("Search/{pcConfigurationId}")]
		public async Task<IActionResult> SearchAsync(int pcConfigurationId, int page = 1)
		{
			var searchQuery = GetSearchQuery();
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

			TComponent? component = await ComponentRepository.GetOneAsync(componentId);

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

			await SetColorsAsync();
			return View(new TComponent());
		}

		[HttpPost("Create/{pcConfigurationId}")]
		public async Task<IActionResult> CreateAsync(int pcConfigurationId,
			PcComponentFormViewModel<TComponent> model)
		{
			if (!await HasAccessToPcConfigurationAsync(pcConfigurationId))
				return StatusCode(403);

			if (model == null)
				return NotFound();

			model.Component.IsDefault = false;

			if (!ModelState.IsValid)
			{
				SetFirstError();
				await SetColorsAsync(model.SelectedColorsId);
				return View(model.Component);
			}

			PcConfiguration? pcConfig = await _pcConfigRepository.GetByIdAsync(pcConfigurationId,
				PcConfigurationIncludes);
			if (pcConfig == null)
				return NotFound();

			await TryDeleteCurrentAsync(pcConfig);

			await ComponentRepository.AddAsync(model.Component);
			ChangeComponent(pcConfig, model.Component);
			await _pcConfigRepository.UpdateAsync(pcConfig);

			await RemoveInvalidColorIdsAsync(model.SelectedColorsId);
			model.Component = await UpdateColorRelationshipsAsync(model.Component,
				model.SelectedColorsId);

			return RedirectToPcSetup(pcConfigurationId);
		}

		[HttpPost("Delete/{pcConfigurationId}")]
		public async Task<IActionResult> DeleteAsync(int pcConfigurationId)
		{
			if (!await HasAccessToPcConfigurationAsync(pcConfigurationId))
				return StatusCode(403);

			PcConfiguration? pcConfig = await _pcConfigRepository.GetByIdAsync(pcConfigurationId,
				PcConfigurationIncludes);
			if (pcConfig == null)
				return NotFound();

			await TryDeleteCurrentAsync(pcConfig);

			return RedirectToPcSetup(pcConfigurationId);
		}

		[HttpGet("Edit/{pcConfigurationId}")]
		public async Task<IActionResult> EditAsync(int pcConfigurationId)
		{
			if (!await HasAccessToPcConfigurationAsync(pcConfigurationId))
				return StatusCode(403);

			PcConfiguration? pcConfig = await _pcConfigRepository.GetByIdAsync(pcConfigurationId,
				PcConfigurationIncludes);
			if (pcConfig == null)
				return NotFound();

			TComponent? component = GetComponent(pcConfig);
			if (component == null)
				return NotFound();
			if (component.IsDefault)
				return StatusCode(403);

			await SetColorsAsync();
			return View(component);
		}

		[HttpPost("Edit/{pcConfigurationId}/{componentId}")]
		public async Task<IActionResult> EditAsync(int pcConfigurationId, int componentId,
			PcComponentFormViewModel<TComponent> model)
		{
			if (!await HasAccessToPcConfigurationAsync(pcConfigurationId))
				return StatusCode(403);

			PcConfiguration? pcConfig = await _pcConfigRepository.GetByIdAsync(pcConfigurationId,
				PcConfigurationIncludes, true);
			if (pcConfig == null)
				return NotFound();
			if (!IsComponentInPcConfig(pcConfig, componentId))
				return StatusCode(403);

			if (model.Component == null)
				return NotFound();
			if (model.Component.IsDefault)
				return StatusCode(403);

			model.Component.SetId(componentId);

			if (!ModelState.IsValid)
			{
				SetFirstError();
				await SetColorsAsync(model.SelectedColorsId);
				return View(model.Component);
			}

			await ComponentRepository.UpdateAsync(model.Component);

			await RemoveInvalidColorIdsAsync(model.SelectedColorsId);
			model.Component = await UpdateColorRelationshipsAsync(model.Component,
				model.SelectedColorsId);

			return RedirectToPcSetup(pcConfigurationId);
		}
		#endregion

		#region Helpers
		protected async Task<bool> TryDeleteCurrentAsync(PcConfiguration pcConfig)
		{
			// Delete the current component if it is not default.
			TComponent? currentComponent = GetComponent(pcConfig);
			if (currentComponent != null && !currentComponent.IsDefault)
			{
				await ComponentRepository.DeleteAsync(currentComponent.Id);
				return true;
			}

			return false;
		}
		#endregion
	}
}