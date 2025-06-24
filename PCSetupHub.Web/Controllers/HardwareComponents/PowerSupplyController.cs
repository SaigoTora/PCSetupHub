using Microsoft.AspNetCore.Mvc;
using PCSetupHub.Data.Models.Attributes;
using PCSetupHub.Data.Models.Hardware;
using PCSetupHub.Data.Models.Relationships;
using PCSetupHub.Data.Repositories.Base;
using PCSetupHub.Data.Repositories.Interfaces.PcConfigurations;
using PCSetupHub.Data.Repositories.Interfaces.Users;

namespace PCSetupHub.Web.Controllers.HardwareComponents
{
	[Route("[Controller]")]
	public class PowerSupplyController : HardwareBaseController<PowerSupply>
	{
		private readonly IRepository<ColorPowerSupply> ColorPowerSupplyRepository;

		protected override string ComponentName => "PowerSupply";
		protected override bool IsComponentColorful => true;
		protected override PcConfigurationIncludes PcConfigurationIncludes =>
			PcConfigurationIncludes.PowerSupply;

		public PowerSupplyController(IPcConfigurationRepository pcConfigRepository,
			IRepository<PowerSupply> powerSupplyRepository, IRepository<Color> colorRepository,
			IRepository<ColorPowerSupply> colorPowerSupplyRepository, IUserRepository userRepository)
			: base(pcConfigRepository, powerSupplyRepository, colorRepository, userRepository)
		{
			ColorPowerSupplyRepository = colorPowerSupplyRepository;
		}

		protected override PowerSupply? GetComponent(PcConfiguration pcConfiguration)
			=> pcConfiguration.PowerSupply;
		protected override void ChangeComponent(PcConfiguration pcConfiguration,
			PowerSupply powerSupply)
		{
			pcConfiguration.ChangePowerSupply(powerSupply);
		}
		protected override void ClearComponent(PcConfiguration pcConfiguration)
			=> pcConfiguration.ClearPowerSupply();
		protected override bool IsComponentInPcConfig(PcConfiguration pcConfig, int powerSupplyId)
			=> pcConfig.PowerSupply?.Id == powerSupplyId;

		protected override async Task<PowerSupply> UpdateColorRelationshipsAsync(
			PowerSupply powerSupply, List<int> colorIds)
		{
			// Getting existing relationships
			var existingRelations = await ColorPowerSupplyRepository
				.GetSomeAsync(cps => cps.PowerSupplyId == powerSupply.Id);

			var existingColorIds = existingRelations.Select(r => r.ColorId).ToHashSet();
			var newColorIds = colorIds.ToHashSet();

			// Removing relationships that no longer exist
			var relationsToRemove = existingRelations
				.Where(r => !newColorIds.Contains(r.ColorId))
				.ToList();

			foreach (var relation in relationsToRemove)
				await ColorPowerSupplyRepository.DeleteAsync(relation);

			// Adding new relationships
			var relationsToAdd = newColorIds
				.Except(existingColorIds)
				.Select(colorId => new ColorPowerSupply(colorId, powerSupply.Id))
				.ToList();

			await ColorPowerSupplyRepository.AddAsync(relationsToAdd);

			// Updating the collection in entity
			var updatedRelations = existingRelations
				.Where(r => newColorIds.Contains(r.ColorId))
				.Concat(relationsToAdd)
				.ToList();

			powerSupply.SetColorPowerSupplies(updatedRelations);

			return powerSupply;
		}
	}
}