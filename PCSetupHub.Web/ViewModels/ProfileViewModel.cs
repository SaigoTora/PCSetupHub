using PCSetupHub.Data.Models.Users;

namespace PCSetupHub.Web.ViewModels
{
	public class ProfileViewModel
	{
		public User User { get; private set; }
		public ContactsPrivacyViewModel ContactsPrivacy { get; private set; }

		public bool AreMessageVisible { get; private set; }
		public bool ArePcConfigVisible { get; private set; }
		public bool IsCommentWritingAllowed { get; private set; }

		public ProfileViewModel(User user, ContactsPrivacyViewModel contactsPrivacy,
			bool areMessageVisible, bool arePcConfigVisible, bool isCommentWritingAllowed)
		{
			User = user;
			ContactsPrivacy = contactsPrivacy;
			AreMessageVisible = areMessageVisible;
			ArePcConfigVisible = arePcConfigVisible;
			IsCommentWritingAllowed = isCommentWritingAllowed;
		}
	}
}