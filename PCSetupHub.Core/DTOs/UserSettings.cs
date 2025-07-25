using System.ComponentModel.DataAnnotations;

using PCSetupHub.Data.Models.Users;

namespace PCSetupHub.Core.DTOs
{
	public class UserSettings
	{
		public string Login { get; private set; } = string.Empty;
		public string AvatarUrl { get; private set; } = string.Empty;
		public int PcConfigurationId { get; private set; }
		public bool HasPassword { get; private set; }

		[Required(ErrorMessage = "Name is required.")]
		[StringLength(64, MinimumLength = 2,
			ErrorMessage = "Name must be between 2 and 64 characters long.")]
		[RegularExpression(@"^[\p{L}]+$",
			ErrorMessage = "Name can only contain letters from any alphabet.")]
		public string Name { get; init; } = string.Empty;

		[Required(ErrorMessage = "Email is required.")]
		[EmailAddress(ErrorMessage = "Invalid email address.")]
		public string Email { get; init; } = string.Empty;

		[StringLength(150)]
		public string? Description { get; init; }

		public int FollowersAccessId { get; init; } = (int)PrivacyLevelType.Everyone;
		public int FollowingsAccessId { get; init; } = (int)PrivacyLevelType.Everyone;
		public int MessagesAccessId { get; init; } = (int)PrivacyLevelType.Everyone;
		public int PcConfigAccessId { get; init; } = (int)PrivacyLevelType.Everyone;

		public UserSettings()
		{ }
		public UserSettings(User user)
		{
			Login = user.Login;
			AvatarUrl = user.AvatarUrl;
			PcConfigurationId = user.PcConfiguration.Id;
			HasPassword = user.HasPassword;

			Name = user.Name;
			Email = user.Email;
			Description = user.Description;
			FollowersAccessId = user.PrivacySetting.FollowersAccessId;
			FollowingsAccessId = user.PrivacySetting.FollowingsAccessId;
			MessagesAccessId = user.PrivacySetting.MessagesAccessId;
			PcConfigAccessId = user.PrivacySetting.PcConfigAccessId;
		}

		public void SetMeta(string login, string avatarUrl, int pcConfigurationId,
			bool hasPassword)
		{
			Login = login;
			AvatarUrl = avatarUrl;
			PcConfigurationId = pcConfigurationId;
			HasPassword = hasPassword;
		}
	}
}