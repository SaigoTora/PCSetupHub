namespace PCSetupHub.Web.ViewModels
{
	public class AssignmentGameInputViewModel
	{
		public int[] ProcessorIds { get; init; } = [];
		public int[] VideoCardIds { get; init; } = [];
		public int[] SelectedAnswers { get; init; } = [];
	}
}