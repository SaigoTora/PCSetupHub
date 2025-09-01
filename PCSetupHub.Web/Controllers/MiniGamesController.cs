using Microsoft.AspNetCore.Mvc;

using PCSetupHub.Data.Models.Hardware;
using PCSetupHub.Data.Repositories.Base;
using PCSetupHub.Data.Repositories.Implementations.PcConfigurations;
using PCSetupHub.Web.ViewModels;

namespace PCSetupHub.Web.Controllers
{
	public class MiniGamesController : Controller
	{
		private readonly IRepository<Processor> _processorRepository;
		private readonly IRepository<VideoCard> _videoCardRepository;
		private readonly Random _random;

		public MiniGamesController(IRepository<Processor> processorRepository,
			IRepository<VideoCard> videoCardRepository, Random random)
		{
			_processorRepository = processorRepository;
			_videoCardRepository = videoCardRepository;
			_random = random;
		}

		[HttpGet]
		public async Task<IActionResult> AssignmentGame()
		{
			const int COMPONENTS_COUNT = 5;
			const int PROCESSOR_MIN_PRICE_DIFFERENCE = 25;
			const int VIDEOCARD_MIN_PRICE_DIFFERENCE = 50;

			int[] processorIds = await _processorRepository
				.GetSomeAsync(p => p.IsDefault && p.Price.HasValue, p => p.Id);
			int[] videoCardIds = await _videoCardRepository
				.GetSomeAsync(v => v.IsDefault && v.Price.HasValue, v => v.Id);

			Processor[] processors = await GetRandomComponents(_processorRepository, processorIds,
				COMPONENTS_COUNT, PROCESSOR_MIN_PRICE_DIFFERENCE);
			VideoCard[] videoCards = await GetRandomComponents(_videoCardRepository, videoCardIds,
				COMPONENTS_COUNT, VIDEOCARD_MIN_PRICE_DIFFERENCE);

			AssignmentGameViewModel model = new(processors, videoCards);
			return View(model);
		}

		private async Task<TComponent[]> GetRandomComponents<TComponent>(
			IRepository<TComponent> componentRepository, int[] componentIds, int resultLength,
			double minPriceDifference = 0)
			where TComponent : HardwareComponent, new()
		{
			if (resultLength > componentIds.Length)
				throw new ArgumentException($"Cannot select {resultLength} components from only " +
					$"{componentIds.Length} available IDs.", nameof(resultLength));

			TComponent[] result = new TComponent[resultLength];
			for (int i = 0; i < resultLength; i++)
				result[i] = new TComponent();
			int resultIndex = 0;

			while (resultIndex < resultLength)
			{
				int randomId = componentIds[_random.Next(0, componentIds.Length)];
				if (result.Any(c => c.Id == randomId))
					continue;

				TComponent? component = await componentRepository.GetOneAsync(randomId);
				if (component == null)
					continue;

				if (minPriceDifference > 0 && result.Any(c => c.Id != 0 && c.Price.HasValue
						&& component.Price.HasValue && Math.Abs(c.Price.GetValueOrDefault() -
							component.Price.GetValueOrDefault()) < minPriceDifference))
					continue;

				result[resultIndex++] = component;
			}

			return result;
		}
	}
}