using Microsoft.EntityFrameworkCore;

using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Data.Repositories.Implementations.PcConfigurations
{
	public class VideoCardRepository(PcSetupContext context)
		: HardwareComponentRepository<VideoCard>(context)
	{
		protected override IQueryable<VideoCard> GetQuery()
		{
			return Context.VideoCards
				.Include(v => v.ColorVideoCards!)
				.ThenInclude(cvc => cvc.Color);
		}
	}
}