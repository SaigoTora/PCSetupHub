using Microsoft.AspNetCore.Mvc;

using PCSetupHub.Core.Extensions;
using PCSetupHub.Core.Interfaces;
using PCSetupHub.Data.Models.Hardware;
using PCSetupHub.Data.Repositories.Base;
using PCSetupHub.Web.ViewModels;

namespace PCSetupHub.Web.Controllers
{
	public class MiniGamesController : Controller
	{
		private const int COMPONENTS_COUNT = 4;

		private readonly IRepository<Processor> _processorRepository;
		private readonly IRepository<VideoCard> _videoCardRepository;
		private readonly IAssignmentService _assignmentService;
		private readonly Random _random;

		public MiniGamesController(IRepository<Processor> processorRepository,
			IRepository<VideoCard> videoCardRepository, IAssignmentService assignmentService,
			Random random)
		{
			_processorRepository = processorRepository;
			_videoCardRepository = videoCardRepository;
			_assignmentService = assignmentService;
			_random = random;
		}

		[HttpGet]
		public async Task<IActionResult> AssignmentGame()
		{
			const int PROCESSOR_MIN_PRICE_DIFFERENCE = 50;
			const int VIDEOCARD_MIN_PRICE_DIFFERENCE = 100;
			const int DEFAULT_SELECT_VALUE = -1;

			int[] processorIds = await _processorRepository
				.GetSomeAsync(p => p.IsDefault && p.Price.HasValue, p => p.Id);
			int[] videoCardIds = await _videoCardRepository
				.GetSomeAsync(v => v.IsDefault && v.Price.HasValue, v => v.Id);

			Processor[] processors = await GetRandomComponents(_processorRepository, processorIds,
				COMPONENTS_COUNT, PROCESSOR_MIN_PRICE_DIFFERENCE);
			VideoCard[] videoCards = await GetRandomComponents(_videoCardRepository, videoCardIds,
				COMPONENTS_COUNT, VIDEOCARD_MIN_PRICE_DIFFERENCE);
			int[] defaultSelects = [.. Enumerable.Repeat(DEFAULT_SELECT_VALUE, COMPONENTS_COUNT)];

			AssignmentGameViewModel model = new(processors, videoCards, defaultSelects);
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> AssignmentGame(AssignmentGameInputViewModel inputModel)
		{
			Processor[] processors = await GetComponentsAsync(_processorRepository,
				inputModel.ProcessorIds);
			VideoCard[] videoCards = await GetComponentsAsync(_videoCardRepository,
				inputModel.VideoCardIds);

			AssignmentGameViewModel outputModel = new(processors, videoCards,
				inputModel.SelectedAnswers);
			if (!ValidateAssignmentInput(inputModel))
				return View(outputModel);

			double[,] costs = new double[processors.Length, videoCards.Length];
			for (int processorIndex = 0; processorIndex < processors.Length; processorIndex++)
				for (int videoCardIndex = 0; videoCardIndex < videoCards.Length; videoCardIndex++)
				{
					costs[processorIndex, videoCardIndex]
						= CalculateCost(processors[processorIndex], videoCards[videoCardIndex]);
				}

			int[] result = _assignmentService.Solve(costs, true);
			outputModel.SetCorrectAnswers(result);

			return View("AssignmentGameResult", outputModel);
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

				if (minPriceDifference > 0 && result.Any(c => c.Id != 0
					&& Math.Abs(c.Price.GetValueOrDefault() - component.Price.GetValueOrDefault())
						< minPriceDifference))
					continue;

				result[resultIndex++] = component;
			}

			return result;
		}
		private static async Task<TComponent[]> GetComponentsAsync<TComponent>(
			IRepository<TComponent> repository, int[] componentIds)
			where TComponent : HardwareComponent, new()
		{
			TComponent[] result = new TComponent[componentIds.Length];

			for (int i = 0; i < componentIds.Length; i++)
			{
				TComponent? component = await repository.GetOneAsync(componentIds[i])
					?? throw new InvalidOperationException($"{typeof(TComponent).Name} " +
						$"with ID {componentIds[i]} not found.");

				if (!component.IsDefault || !component.Price.HasValue)
					throw new InvalidOperationException($"{typeof(TComponent).Name} " +
						$"with ID {componentIds[i]} is not valid for the game.");

				result[i] = component;
			}

			return result;
		}
		private bool ValidateAssignmentInput(AssignmentGameInputViewModel inputModel)
		{
			if (inputModel.ProcessorIds.Length != COMPONENTS_COUNT)
			{
				ModelState.AddModelError(nameof(inputModel.ProcessorIds),
					$"You must select exactly {COMPONENTS_COUNT} processors.");
				this.SetFirstError();
				return false;
			}
			if (inputModel.VideoCardIds.Length != COMPONENTS_COUNT)
			{
				ModelState.AddModelError(nameof(inputModel.VideoCardIds),
					$"You must select exactly {COMPONENTS_COUNT} video cards.");
				this.SetFirstError();
				return false;
			}

			HashSet<int> uniqueSelectedAnswers = [.. inputModel.SelectedAnswers];
			if (uniqueSelectedAnswers.Count != COMPONENTS_COUNT
				|| uniqueSelectedAnswers.Any(a => a < 0 || a >= COMPONENTS_COUNT))
			{
				ModelState.AddModelError(nameof(inputModel.SelectedAnswers),
					$"You must select {COMPONENTS_COUNT} unique components (no duplicates).");
				this.SetFirstError();
				return false;
			}

			return true;
		}
		private static double CalculateCost(Processor processor, VideoCard videoCard)
		{
			double cpuPrice = processor.Price ?? 0;
			double gpuPrice = videoCard.Price ?? 0;
			double cpuPerf = processor.CoreCount * (processor.BoostClock ?? processor.CoreClock);
			double gpuPerf = videoCard.Memory
				* ((videoCard.BoostClock ?? videoCard.CoreClock ?? 0) / 1000);

			// Weights
			const double PRICE_WEIGHT = 2.0;
			const double COMPATIBILITY_WEIGHT = 5.0;

			double priceRatio = Math.Min(cpuPrice, gpuPrice) / (Math.Max(cpuPrice, gpuPrice) + 1);
			double compatibilityRatio = Math.Min(cpuPerf, gpuPerf)
				/ (Math.Max(cpuPerf, gpuPerf) + 1);

			const double SCORE_MULTIPLIER = 100.0;
			double score = (compatibilityRatio * COMPATIBILITY_WEIGHT + priceRatio * PRICE_WEIGHT)
				* SCORE_MULTIPLIER;

			return score;
		}
	}
}