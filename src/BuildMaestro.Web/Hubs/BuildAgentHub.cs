using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace BuildMaestro.Web.Hubs
{
    [HubName("buildagent")]
    public class BuildAgentHub : Hub
    {
        public void SendMessage(string chatMessage)
        {
            Clients.All.SendMessage(chatMessage);
        }
    }
}