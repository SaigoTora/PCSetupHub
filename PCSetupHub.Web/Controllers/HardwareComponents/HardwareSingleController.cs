using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PCSetupHub.Core.Extensions;
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

		protected readonly ILogger<HardwareSingleController<TComponent>> Logger;
		private readonly IPcConfigurationRepository _pcConfigRepository;

		protected HardwareSingleController(ILogger<HardwareSingleController<TComponent>> logger,
			IPcConfigurationRepository pcConfigRepository,
			IRepository<TComponent> componentRepository, IRepository<Color> colorRepository,
			IUserRepository userRepository)
			: base(componentRepository, colorRepository, userRepository)
		{
			Logger = logger;
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
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> Search(int pcConfigurationId, int page = 1)
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
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> Select(int pcConfigurationId, int componentId)
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

			try
			{
				ChangeComponent(pcConfig, component);
				await _pcConfigRepository.UpdateAsync(pcConfig);
				Logger.LogInformation("Selected {ComponentName} with id {ComponentId} to config " +
					"{ConfigId}", ComponentName, componentId, pcConfigurationId);
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, "Failed to select {ComponentName} with id {ComponentId} " +
					"to config {ConfigId}", ComponentName, componentId, pcConfigurationId);
				throw;
			}

			return RedirectToPcSetup(pcConfigurationId);
		}

		[HttpPost("Clear/{pcConfigurationId}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> Clear(int pcConfigurationId)
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
			if (component == null)
				return NotFound();

			try
			{
				ClearComponent(pcConfig);
				await _pcConfigRepository.UpdateAsync(pcConfig);
				Logger.LogInformation("Cleared {ComponentName} with id {ComponentId} " +
					"from config {ConfigId}", ComponentName, component.Id, pcConfigurationId);

			}
			catch (Exception ex)
			{
				Logger.LogError(ex, "Failed to clear {ComponentName} with id {ComponentId} " +
					"from config {ConfigId}", ComponentName, component.Id, pcConfigurationId);
				throw;
			}

			return RedirectToPcSetup(pcConfigurationId);
		}

		[HttpGet("Create/{pcConfigurationId}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> Create(int pcConfigurationId)
		{
			if (!await HasAccessToPcConfigurationAsync(pcConfigurationId))
				return StatusCode(403);

			await SetColorsAsync();
			return View(new TComponent());
		}

		[HttpPost("Create/{pcConfigurationId}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> Create(int pcConfigurationId,
			PcComponentFormViewModel<TComponent> model)
		{
			if (!await HasAccessToPcConfigurationAsync(pcConfigurationId))
				return StatusCode(403);

			if (model == null)
				return NotFound();

			model.Component.IsDefault = false;

			if (!ModelState.IsValid)
			{
				this.SetFirstError();
				await SetColorsAsync(model.SelectedColorsId);
				return View(model.Component);
			}

			PcConfiguration? pcConfig = await _pcConfigRepository.GetByIdAsync(pcConfigurationId,
				PcConfigurationIncludes);
			if (pcConfig == null)
				return NotFound();

			await TryDeleteCurrentAsync(pcConfig);

			try
			{
				await ComponentRepository.AddAsync(model.Component);
				ChangeComponent(pcConfig, model.Component);
				await _pcConfigRepository.UpdateAsync(pcConfig);
				Logger.LogInformation("Created and linked {ComponentName} with id {ComponentId} " +
					"to config {ConfigId}", ComponentName, model.Component.Id, pcConfigurationId);
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, "Failed to create and link {ComponentName} " +
					"to config {ConfigId}", ComponentName, pcConfigurationId);
				throw;
			}

			await RemoveInvalidColorIdsAsync(model.SelectedColorsId);
			model.Component = await UpdateColorRelationshipsAsync(model.Component,
				model.SelectedColorsId);

			return RedirectToPcSetup(pcConfigurationId);
		}

		[HttpPost("Delete/{pcConfigurationId}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> Delete(int pcConfigurationId)
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

		[HttpGet("Edit/{pcConfigurationId}/{componentId}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> Edit(int pcConfigurationId)
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
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> Edit(int pcConfigurationId, int componentId,
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
				this.SetFirstError();
				await SetColorsAsync(model.SelectedColorsId);
				return View(model.Component);
			}

			try
			{
				await ComponentRepository.UpdateAsync(model.Component);
				Logger.LogInformation("Updated {ComponentName} with id {ComponentId} in config " +
					"{ConfigId}", ComponentName, componentId, pcConfigurationId);
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, "Failed to update {ComponentName} with id {ComponentId} " +
					"in config {ConfigId}", ComponentName, componentId, pcConfigurationId);
				throw;
			}

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
				try
				{
					await ComponentRepository.DeleteAsync(currentComponent.Id);
					Logger.LogInformation("Deleted {ComponentName} with id {ComponentId}",
						ComponentName, currentComponent.Id);
				}
				catch (Exception ex)
				{
					Logger.LogError(ex, "Failed to delete {ComponentName} with id {ComponentId}",
						ComponentName, currentComponent.Id);
					throw;
				}
				return true;
			}

			return false;
		}
		#endregion
	}
}