using Microsoft.AspNetCore.SignalR;

namespace PCSetupHub.Web.Hubs
{
	public class ChatHub : Hub
	{
		public async Task JoinToChat(string chatName)
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, chatName);
		}

		public async Task SendMessage(string chatName, string user, string message)
		{
			await Clients
				.Group(chatName)
				.SendAsync("ReceiveMessage", $"{user} says - {message}	from chat {chatName}");
		}
	}
}