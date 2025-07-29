using PCSetupHub.Data.Models.Users;

namespace PCSetupHub.Web.ViewModels
{
	public class ContactsViewModel : PaginationViewModel
	{
		public ICollection<User>? Contacts { get; private set; }
		public string Login { get; private set; } = string.Empty;
		public ContactsPrivacyViewModel? ContactsPrivacy { get; private set; }

		public ContactsViewModel()
			: base()
		{ }
		public ContactsViewModel(ICollection<User> contacts, string login, string? searchQuery,
			int page, int totalItems, string actionName, int pageSize, string searchQueryName)
			: base(page, totalItems, actionName, pageSize, "page", searchQuery, searchQueryName)
		{
			Contacts = contacts;
			Login = login;
		}
		public ContactsViewModel(ICollection<User> contacts, string login,
			ContactsPrivacyViewModel contactsPrivacy, string? searchQuery, int page,
			int totalItems, string actionName, int pageSize, string searchQueryName)
			: this(contacts, login, searchQuery, page, totalItems, actionName, pageSize,
				  searchQueryName)
		{
			ContactsPrivacy = contactsPrivacy;
		}
	}
}