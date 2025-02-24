using PCSetupHub.Models.Base;

namespace PCSetupHub.Models.Users
{
	public class Comment : BaseEntity
	{
		public int UserID { get; private set; }
		public int? CommentatorID { get; private set; }
		public User? User { get; private set; }
		public User? Commentator { get; private set; }
		public string Text { get; private set; } = string.Empty;
		public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

		public Comment() { }
		public Comment(int userID, int commentatorID, string text)
		{
			UserID = userID;
			CommentatorID = commentatorID;
			Text = text;
		}
	}
}