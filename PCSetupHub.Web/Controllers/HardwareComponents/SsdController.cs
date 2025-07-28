using Microsoft.AspNetCore.Mvc;

using PCSetupHub.Data.Models.Attributes;
using PCSetupHub.Data.Models.Hardware;
using PCSetupHub.Data.Models.Relationships;
using PCSetupHub.Data.Repositories.Base;
using PCSetupHub.Data.Repositories.Interfaces.Users;

namespace PCSetupHub.Web.Controllers.HardwareComponents
{
	[Route("[Controller]")]
	public class SsdController : HardwareMultiController<Ssd>
	{
		protected override string ComponentName => "Ssd";
		protected override bool IsComponentColorful => false;
		protected override int MaxAllowedCount => PcConfiguration.MAX_SSD_COUNT;

		private readonly IRepository<PcConfigurationSsd> _pcConfigSsdRepository;

		public SsdController(ILogger<SsdController> logger, IRepository<Ssd> ssdRepository,
			IRepository<PcConfigurationSsd> pcConfigSsdRepository,
			IRepository<Color> colorRepository, IUserRepository userRepository)
			: base(logger, ssdRepository, colorRepository, userRepository)
		{
			_pcConfigSsdRepository = pcConfigSsdRepository;
		}

		protected override async Task<List<int>> GetRelatedComponentIdsAsync(int pcConfigId)
		{
			return [.. (await _pcConfigSsdRepository
				.GetSomeAsync(r => r.PcConfigurationId == pcConfigId))
				.Select(r => r.SsdId)];
		}
		protected override async Task UpdateRelationAsync(int pcConfigId, int currentId, int newId)
		{
			var item = (await _pcConfigSsdRepository
				.GetSomeAsync(r => r.PcConfigurationId == pcConfigId && r.SsdId == currentId))
				.FirstOrDefault();

			if (item == null)
				return;

			item.SsdId = newId;
			await _pcConfigSsdRepository.UpdateAsync(item);
		}
		protected override async Task ClearRelationAsync(int pcConfigId, int ssdId)
		{
			var item = (await _pcConfigSsdRepository
				.GetSomeAsync(r => r.PcConfigurationId == pcConfigId && r.SsdId == ssdId))
				.FirstOrDefault();

			if (item == null)
				return;

			await _pcConfigSsdRepository.DeleteAsync(item);
		}
		protected override async Task CreateRelationAsync(int pcConfigId, int ssdId)
		{
			PcConfigurationSsd newItem = new(pcConfigId, ssdId);
			await _pcConfigSsdRepository.AddAsync(newItem);
		}
	}
}