namespace PCSetupHub.Web.ViewModels
{
	public class ContactsPrivacyViewModel
	{
		public bool AreFriendsVisible { get; private set; }
		public bool AreFollowersVisible { get; private set; }
		public bool AreFollowingsVisible { get; private set; }

		public ContactsPrivacyViewModel(bool areFriendsVisible, bool areFollowersVisible,
			bool areFollowingsVisible)
		{
			AreFriendsVisible = areFriendsVisible;
			AreFollowersVisible = areFollowersVisible;
			AreFollowingsVisible = areFollowingsVisible;
		}
	}
}