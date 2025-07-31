using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

using PCSetupHub.Core.Extensions;
using PCSetupHub.Data.Models.Attributes;
using PCSetupHub.Data.Models.Hardware;
using PCSetupHub.Data.Repositories.Base;
using PCSetupHub.Data.Repositories.Interfaces.Users;
using PCSetupHub.Web.ViewModels;

namespace PCSetupHub.Web.Controllers.HardwareComponents
{
	public abstract class HardwareMultiController<TComponent> : HardwareBaseController<TComponent>
		where TComponent : HardwareComponent, new()
	{
		protected abstract int MaxAllowedCount { get; }

		protected readonly ILogger<HardwareMultiController<TComponent>> Logger;

		protected HardwareMultiController(ILogger<HardwareMultiController<TComponent>> logger,
			IRepository<TComponent> componentRepository, IRepository<Color> colorRepository,
			IUserRepository userRepository)
			: base(componentRepository, colorRepository, userRepository)
		{
			Logger = logger;
		}

		#region Abstract methods
		protected abstract Task<List<int>> GetRelatedComponentIdsAsync(int pcConfigId);
		protected abstract Task UpdateRelationAsync(int pcConfigId, int currentId, int newId);
		protected abstract Task ClearRelationAsync(int pcConfigId, int componentId);
		protected abstract Task CreateRelationAsync(int pcConfigId, int componentId);
		#endregion

		#region Virtual methods
		protected virtual Task<TComponent> UpdateColorRelationshipsAsync(TComponent component,
			List<int> colorIds)
			=> Task.FromResult(component);
		#endregion

		#region Action methods
		[HttpGet("Search/{pcConfigurationId}/{componentId}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> Search(int pcConfigurationId, int componentId,
			int page = 1)
		{
			var searchQuery = GetSearchQuery();
			if (!await HasAccessToPcConfigurationAsync(pcConfigurationId))
				return StatusCode(403);

			List<int> relatedComponentIds = await GetRelatedComponentIdsAsync(pcConfigurationId);
			if (!relatedComponentIds.Any(i => i == componentId))
				return StatusCode(403);

			var filteredComponents = await GetFilteredComponentsAsync(searchQuery);
			int totalItems = filteredComponents.Count;

			SearchComponentViewModel model = new(pcConfigurationId, searchQuery,
				componentId, ComponentName, page, totalItems, relatedComponentIds);

			model.Components = GetPagedItems(filteredComponents, model.Page, model.PageSize);
			return View(model);
		}

		[HttpPost("Select/{pcConfigurationId}/{currentComponentId}/{componentId}")]
		[EnableRateLimiting("PcConfiguration")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> Select(int pcConfigurationId, int currentComponentId,
			int componentId)
		{
			if (!await HasAccessToPcConfigurationAsync(pcConfigurationId))
				return StatusCode(403);

			List<int> relatedComponentIds = await GetRelatedComponentIdsAsync(pcConfigurationId);
			if (!relatedComponentIds.Any(i => i == currentComponentId))
				return StatusCode(403);

			TComponent? component = await ComponentRepository.GetOneAsync(componentId);
			if (component == null)
				return NotFound();
			if (!component.IsDefault)
				return StatusCode(403);

			await TryDeleteCurrentAsync(currentComponentId);

			try
			{
				await UpdateRelationAsync(pcConfigurationId, currentComponentId, componentId);
				Logger.LogInformation("Selected {ComponentName} with id {ComponentId} " +
					"for config {ConfigId}, replacing {CurrentComponentId}",
					ComponentName, componentId, pcConfigurationId, currentComponentId);
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, "Failed to assign {ComponentName} with id {ComponentId} " +
					"to config {ConfigId}, replacing {CurrentComponentId}", ComponentName,
					componentId, pcConfigurationId, currentComponentId);
				throw;
			}

			return RedirectToPcSetup(pcConfigurationId);
		}

		[HttpPost("Clear/{pcConfigurationId}/{componentId}")]
		[EnableRateLimiting("PcConfiguration")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> Clear(int pcConfigurationId, int componentId)
		{
			if (!await HasAccessToPcConfigurationAsync(pcConfigurationId))
				return StatusCode(403);

			List<int> relatedComponentIds = await GetRelatedComponentIdsAsync(pcConfigurationId);
			if (!relatedComponentIds.Any(i => i == componentId))
				return StatusCode(403);

			TComponent? component = await ComponentRepository.GetOneAsync(componentId);
			if (component != null && !component.IsDefault)
				return StatusCode(403);

			try
			{
				await ClearRelationAsync(pcConfigurationId, componentId);
				Logger.LogInformation("Cleared {ComponentName} with id {ComponentId} " +
					"from config {ConfigId}", ComponentName, componentId, pcConfigurationId);

			}
			catch (Exception ex)
			{
				Logger.LogError(ex, "Failed to clear {ComponentName} with id {ComponentId} " +
					"from config {ConfigId}", ComponentName, componentId, pcConfigurationId);
				throw;
			}

			return RedirectToPcSetup(pcConfigurationId);
		}

		[HttpGet("Search/{pcConfigurationId}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> Search(int pcConfigurationId, int page = 1)
		{
			var searchQuery = GetSearchQuery();
			if (!await HasAccessToPcConfigurationAsync(pcConfigurationId))
				return StatusCode(403);

			List<int> relatedComponentIds = await GetRelatedComponentIdsAsync(pcConfigurationId);
			if (!IsComponentCountUnderLimit(relatedComponentIds.Count))
				return Conflict();

			var filteredComponents = await GetFilteredComponentsAsync(searchQuery);
			int totalItems = filteredComponents.Count;

			SearchComponentViewModel model = new(pcConfigurationId, searchQuery,
				null, ComponentName, page, totalItems, relatedComponentIds);

			model.Components = GetPagedItems(filteredComponents, model.Page, model.PageSize);
			return View(model);
		}

		[HttpPost("Select/{pcConfigurationId}/{componentId}")]
		[EnableRateLimiting("PcConfiguration")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> Select(int pcConfigurationId, int componentId)
		{
			if (!await HasAccessToPcConfigurationAsync(pcConfigurationId))
				return StatusCode(403);

			if (!await IsComponentCountUnderLimitAsync(pcConfigurationId))
				return Conflict();

			TComponent? component = await ComponentRepository.GetOneAsync(componentId);
			if (component == null)
				return NotFound();
			if (!component.IsDefault)
				return StatusCode(403);

			try
			{
				await CreateRelationAsync(pcConfigurationId, componentId);
				Logger.LogInformation("Added {ComponentName} with id {ComponentId} to config " +
					"{ConfigId}", ComponentName, componentId, pcConfigurationId);
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, "Failed to add {ComponentName} with id {ComponentId} " +
					"to config {ConfigId}", ComponentName, componentId, pcConfigurationId);
				throw;
			}

			return RedirectToPcSetup(pcConfigurationId);
		}

		[HttpGet("Create/{pcConfigurationId}/{currentComponentId}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> Create(int pcConfigurationId, int currentComponentId)
		{
			if (!await HasAccessToPcConfigurationAsync(pcConfigurationId))
				return StatusCode(403);

			await SetColorsAsync();
			return View(new TComponent());
		}

		[HttpPost("Create/{pcConfigurationId}/{currentComponentId}")]
		[EnableRateLimiting("PcConfiguration")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> Create(int pcConfigurationId, int currentComponentId,
			PcComponentFormViewModel<TComponent> model)
		{
			if (!await HasAccessToPcConfigurationAsync(pcConfigurationId))
				return StatusCode(403);

			if (model == null)
				return NotFound();

			model.Component.IsDefault = false;

			if (await this.HandleInvalidModelStateAsync(model.Component,
				() => SetColorsAsync(model.SelectedColorsId)) is ViewResult errorResult)
				return errorResult;

			List<int> relatedComponentIds = await GetRelatedComponentIdsAsync(pcConfigurationId);
			if (!relatedComponentIds.Any(i => i == currentComponentId))
				return StatusCode(403);

			await TryDeleteCurrentAsync(currentComponentId);

			try
			{
				TComponent newComponent = await ComponentRepository.AddAsync(model.Component);
				await UpdateRelationAsync(pcConfigurationId, currentComponentId, newComponent.Id);
				Logger.LogInformation("Created {ComponentName} with id {ComponentId} and " +
					"assigned to config {ConfigId}, replacing {CurrentComponentId}",
					ComponentName, newComponent.Id, pcConfigurationId, currentComponentId);
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, "Failed to create and assign {ComponentName} to config " +
					"{ConfigId}, replacing {CurrentComponentId}", ComponentName, pcConfigurationId,
					currentComponentId);
				throw;
			}

			await RemoveInvalidColorIdsAsync(model.SelectedColorsId);
			model.Component = await UpdateColorRelationshipsAsync(model.Component,
				model.SelectedColorsId);

			return RedirectToPcSetup(pcConfigurationId);
		}

		[HttpPost("Delete/{pcConfigurationId}/{componentId}")]
		[EnableRateLimiting("PcConfiguration")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> Delete(int pcConfigurationId, int componentId)
		{
			if (!await HasAccessToPcConfigurationAsync(pcConfigurationId))
				return StatusCode(403);

			List<int> relatedComponentIds = await GetRelatedComponentIdsAsync(pcConfigurationId);
			if (!relatedComponentIds.Any(i => i == componentId))
				return StatusCode(403);

			await TryDeleteCurrentAsync(componentId);

			return RedirectToPcSetup(pcConfigurationId);
		}

		[HttpGet("Edit/{pcConfigurationId}/{componentId}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> Edit(int pcConfigurationId, int componentId)
		{
			if (!await HasAccessToPcConfigurationAsync(pcConfigurationId))
				return StatusCode(403);

			List<int> relatedComponentIds = await GetRelatedComponentIdsAsync(pcConfigurationId);
			if (!relatedComponentIds.Any(i => i == componentId))
				return StatusCode(403);

			TComponent? component = await ComponentRepository.GetOneAsync(componentId);
			if (component == null)
				return NotFound();
			if (component.IsDefault)
				return StatusCode(403);

			await SetColorsAsync();
			return View(component);
		}

		[HttpPost("Edit/{pcConfigurationId}/{componentId}")]
		[EnableRateLimiting("PcConfiguration")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> Edit(int pcConfigurationId, int componentId,
			PcComponentFormViewModel<TComponent> model)
		{
			if (!await HasAccessToPcConfigurationAsync(pcConfigurationId))
				return StatusCode(403);

			List<int> relatedComponentIds = await GetRelatedComponentIdsAsync(pcConfigurationId);
			if (!relatedComponentIds.Any(i => i == componentId))
				return StatusCode(403);

			if (model.Component == null)
				return NotFound();
			if (model.Component.IsDefault)
				return StatusCode(403);

			model.Component.SetId(componentId);

			if (await this.HandleInvalidModelStateAsync(model.Component,
				() => SetColorsAsync(model.SelectedColorsId)) is ViewResult errorResult)
				return errorResult;

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

		[HttpGet("Create/{pcConfigurationId}")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> Create(int pcConfigurationId)
		{
			if (!await HasAccessToPcConfigurationAsync(pcConfigurationId))
				return StatusCode(403);

			if (!await IsComponentCountUnderLimitAsync(pcConfigurationId))
				return Conflict();

			await SetColorsAsync();
			return View(new TComponent());
		}

		[HttpPost("Create/{pcConfigurationId}")]
		[EnableRateLimiting("PcConfiguration")]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		public async Task<IActionResult> Create(int pcConfigurationId,
			PcComponentFormViewModel<TComponent> model)
		{
			if (!await HasAccessToPcConfigurationAsync(pcConfigurationId))
				return StatusCode(403);

			if (!await IsComponentCountUnderLimitAsync(pcConfigurationId))
				return Conflict();

			if (model == null)
				return NotFound();

			model.Component.IsDefault = false;

			if (await this.HandleInvalidModelStateAsync(model.Component,
				() => SetColorsAsync(model.SelectedColorsId)) is ViewResult errorResult)
				return errorResult;

			try
			{
				TComponent newComponent = await ComponentRepository.AddAsync(model.Component);
				await CreateRelationAsync(pcConfigurationId, newComponent.Id);
				Logger.LogInformation("Created and linked {ComponentName} with id {ComponentId} " +
					"to config {ConfigId}", ComponentName, newComponent.Id, pcConfigurationId);
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
		#endregion

		#region Helpers
		protected async Task<bool> TryDeleteCurrentAsync(int currentComponentId)
		{
			TComponent? currentComponent = await ComponentRepository
				.GetOneAsync(currentComponentId);

			// Delete the current component if it is not default.
			if (currentComponent != null && !currentComponent.IsDefault)
			{
				try
				{
					await ComponentRepository.DeleteAsync(currentComponent.Id);
					Logger.LogInformation("Deleted {ComponentName} with id {ComponentId}",
						ComponentName, currentComponentId);
				}
				catch (Exception ex)
				{
					Logger.LogError(ex, "Failed to delete {ComponentName} with id {ComponentId}",
						ComponentName, currentComponentId);
					throw;
				}
				return true;
			}

			return false;
		}
		protected bool IsComponentCountUnderLimit(int count) => count < MaxAllowedCount;
		protected async Task<bool> IsComponentCountUnderLimitAsync(int pcConfigurationId)
		{
			List<int> relatedComponentIds = await GetRelatedComponentIdsAsync(pcConfigurationId);
			return IsComponentCountUnderLimit(relatedComponentIds.Count);
		}
		#endregion
	}
}