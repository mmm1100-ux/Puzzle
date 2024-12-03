using Microsoft.AspNetCore.SignalR;

namespace Puzzle.Helper
{
    public class SignalRHub : Hub
    {
        public void Notification(string title, string body, string url)
        {
            Clients.All.SendAsync("Notification", title, body, url).GetAwaiter();
        }
    }
}
