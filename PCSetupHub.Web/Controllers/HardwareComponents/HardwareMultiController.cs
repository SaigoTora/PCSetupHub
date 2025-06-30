using Microsoft.AspNetCore.Mvc;

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
		protected HardwareMultiController(IRepository<TComponent> componentRepository,
			IRepository<Color> colorRepository, IUserRepository userRepository)
			: base(componentRepository, colorRepository, userRepository)
		{ }

		protected abstract Task<List<int>> GetRelatedComponentIdsAsync(int pcConfigId);
		protected abstract Task UpdateRelationAsync(int pcConfigId, int currentId, int newId);
		protected abstract Task ClearRelationAsync(int pcConfigId, int componentId);
		protected abstract Task CreateRelationAsync(int pcConfigId, int componentId);

		#region Action methods
		[HttpGet("Search/{pcConfigurationId}/{componentId}")]
		public async Task<IActionResult> SearchAsync(int pcConfigurationId, int componentId,
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
		public async Task<IActionResult> SelectAsync(int pcConfigurationId, int currentComponentId,
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
			await UpdateRelationAsync(pcConfigurationId, currentComponentId, componentId);

			return RedirectToPcSetup(pcConfigurationId);
		}

		[HttpPost("Clear/{pcConfigurationId}/{componentId}")]
		public async Task<IActionResult> ClearAsync(int pcConfigurationId, int componentId)
		{
			if (!await HasAccessToPcConfigurationAsync(pcConfigurationId))
				return StatusCode(403);

			List<int> relatedComponentIds = await GetRelatedComponentIdsAsync(pcConfigurationId);
			if (!relatedComponentIds.Any(i => i == componentId))
				return StatusCode(403);

			TComponent? component = await ComponentRepository.GetOneAsync(componentId);
			if (component != null && !component.IsDefault)
				return StatusCode(403);

			await ClearRelationAsync(pcConfigurationId, componentId);

			return RedirectToPcSetup(pcConfigurationId);
		}
		#endregion

		[HttpGet("Search/{pcConfigurationId}")]
		public async Task<IActionResult> SearchAsync(int pcConfigurationId, int page = 1)
		{
			var searchQuery = GetSearchQuery();
			if (!await HasAccessToPcConfigurationAsync(pcConfigurationId))
				return StatusCode(403);

			List<int> relatedComponentIds = await GetRelatedComponentIdsAsync(pcConfigurationId);

			var filteredComponents = await GetFilteredComponentsAsync(searchQuery);
			int totalItems = filteredComponents.Count;

			SearchComponentViewModel model = new(pcConfigurationId, searchQuery,
				null, ComponentName, page, totalItems, relatedComponentIds);

			model.Components = GetPagedItems(filteredComponents, model.Page, model.PageSize);
			return View(model);
		}

		[HttpPost("Select/{pcConfigurationId}/{componentId}")]
		public async Task<IActionResult> SelectAsync(int pcConfigurationId, int componentId)
		{
			if (!await HasAccessToPcConfigurationAsync(pcConfigurationId))
				return StatusCode(403);

			TComponent? component = await ComponentRepository.GetOneAsync(componentId);
			if (component == null)
				return NotFound();
			if (!component.IsDefault)
				return StatusCode(403);

			await CreateRelationAsync(pcConfigurationId, componentId);

			return RedirectToPcSetup(pcConfigurationId);
		}

		#region Helpers
		protected async Task<bool> TryDeleteCurrentAsync(int currentComponentId)
		{
			TComponent? currentComponent = await ComponentRepository
				.GetOneAsync(currentComponentId);

			// Delete the current component if it is not default.
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