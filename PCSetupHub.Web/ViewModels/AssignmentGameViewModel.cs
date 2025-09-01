using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Web.ViewModels
{
	public class AssignmentGameViewModel
	{
		public Processor[] Processors { get; private set; } = [];
		public VideoCard[] VideoCards { get; private set; } = [];

		public int[] SelectedAnswers { get; private set; } = [];

		public AssignmentGameViewModel(Processor[] processors, VideoCard[] videoCards)
		{
			Processors = processors;
			VideoCards = videoCards;
		}
	}
}