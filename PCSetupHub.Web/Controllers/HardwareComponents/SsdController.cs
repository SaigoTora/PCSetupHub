using Microsoft.AspNetCore.Mvc;

using PCSetupHub.Data.Models.Attributes;
using PCSetupHub.Data.Models.Hardware;
using PCSetupHub.Data.Models.Relationships;
using PCSetupHub.Data.Repositories.Base;
using PCSetupHub.Data.Repositories.Interfaces.Users;

namespace PCSetupHub.Web.Controllers.HardwareComponents
{
	public class SsdController : HardwareMultiController<Ssd>
	{
		protected override string ComponentName => "Ssd";
		protected override bool IsComponentColorful => false;

		private readonly IRepository<PcConfigurationSsd> _pcConfigSsdRepository;

		public SsdController(IRepository<Ssd> ssdRepository,
			IRepository<PcConfigurationSsd> pcConfigSsdRepository,
			IRepository<Color> colorRepository, IUserRepository userRepository)
			: base(ssdRepository, colorRepository, userRepository)
		{
			_pcConfigSsdRepository = pcConfigSsdRepository;
		}

		protected override async Task<List<int>> GetRelatedComponentIdsAsync(int pcConfigId)
		{
			return [.. (await _pcConfigSsdRepository
				.GetSomeAsync(r => r.PcConfigurationId == pcConfigId))
				.Select(r => r.SsdId)];
		}
		protected override async Task UpdateComponentRelationAsync(int pcConfigId, int currentId,
			int newId)
		{
			var item = (await _pcConfigSsdRepository
				.GetSomeAsync(r => r.PcConfigurationId == pcConfigId && r.SsdId == currentId))
				.FirstOrDefault();

			if (item == null)
				return;

			item.ChangeSsdId(newId);
			await _pcConfigSsdRepository.UpdateAsync(item);
		}

		public IActionResult Add()
		{
			return Ok($"Add from {GetType().Name}");
		}
		public IActionResult Clear()
		{
			return Ok($"Clear from {GetType().Name}");
		}
		public IActionResult Edit()
		{
			return Ok($"Edit from {GetType().Name}");
		}
		public IActionResult Delete()
		{
			return Ok($"Delete from {GetType().Name}");
		}
	}
}