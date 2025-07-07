using PCSetupHub.Data.Models.Users;

namespace PCSetupHub.Web.ViewModels
{
	public class ContactsViewModel : PaginationViewModel
	{
		public ICollection<User>? Contacts { get; private set; }

		public ContactsViewModel()
			: base()
		{ }
		public ContactsViewModel(ICollection<User> contacts, string? searchQuery,
			int page, int totalItems, string actionName, int pageSize, string searchQueryName)
			: base(page, totalItems, actionName, pageSize, "page", searchQuery, searchQueryName)
		{
			Contacts = contacts;
		}
	}
}