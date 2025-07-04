using System.ComponentModel.DataAnnotations;

using PCSetupHub.Data.Models.Base;

namespace PCSetupHub.Data.Models.Users
{
	public class Comment : BaseEntity
	{
		public int UserId { get; private set; }
		public int? CommentatorId { get; private set; }
		public User? User { get; private set; }
		public User? Commentator { get; private set; }

		[Required(ErrorMessage = "Text is required.")]
		[StringLength(512, MinimumLength = 1,
			ErrorMessage = "Text must be between 1 and 512 characters long.")]
		public string Text { get; private set; } = string.Empty;
		public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

		public Comment() { }
		public Comment(int userId, int commentatorId, string text)
		{
			UserId = userId;
			CommentatorId = commentatorId;
			Text = text;
		}

		public void ClearUser() => User = null;
		public void ClearCommentator() => Commentator = null;
	}
}