using PCSetupHub.Data.Models.Users;

namespace PCSetupHub.Web.ViewModels
{
	public class ProfileViewModel
	{
		public User User { get; private set; }

		public bool FollowersVisibility { get; private set; }
		public bool FollowingsVisibility { get; private set; }
		public bool MessageVisibility { get; private set; }
		public bool PcConfigVisibility { get; private set; }

		public ProfileViewModel(User user, bool followersVisibility, bool followingsVisibility,
			bool messageVisibility, bool pcConfigVisibility)
		{
			User = user;
			FollowersVisibility = followersVisibility;
			FollowingsVisibility = followingsVisibility;
			MessageVisibility = messageVisibility;
			PcConfigVisibility = pcConfigVisibility;
		}
	}
}