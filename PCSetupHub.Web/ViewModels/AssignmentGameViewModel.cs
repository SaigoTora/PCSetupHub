using PCSetupHub.Data.Models.Hardware;

namespace PCSetupHub.Web.ViewModels
{
	public class AssignmentGameViewModel
	{
		public Processor[] Processors { get; private set; } = [];
		public VideoCard[] VideoCards { get; private set; } = [];
		public int[] SelectedAnswers { get; private set; } = [];
		public int[]? CorrectAnswers { get; private set; }

		public int TotalQuestions => Math.Min(Processors.Length, VideoCards.Length);
		public int CorrectCount { get; private set; }
		public double CorrectPercent { get; private set; }

		public AssignmentGameViewModel(Processor[] processors, VideoCard[] videoCards,
			int[] selectedAnswers)
		{
			Processors = processors;
			VideoCards = videoCards;
			SelectedAnswers = selectedAnswers;
		}

		public void SetCorrectAnswers(int[] correctAnswers)
		{
			CorrectAnswers = correctAnswers;
			SetCorrectCount();
			SetCorrectPercent();
		}
		private void SetCorrectCount()
		{
			CorrectCount = 0;
			if (SelectedAnswers == null || CorrectAnswers == null)
				return;

			int length = Math.Min(SelectedAnswers.Length, CorrectAnswers.Length);

			for (int i = 0; i < length; i++)
				if (SelectedAnswers[i] == CorrectAnswers[i])
					CorrectCount++;
		}
		private void SetCorrectPercent()
		{
			CorrectPercent = 0;
			if (TotalQuestions == 0)
				return;

			CorrectPercent = (double)CorrectCount / TotalQuestions * 100.0;
		}
	}
}