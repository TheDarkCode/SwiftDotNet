using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Swift.Net.WebAPI.Hubs
{
    [HubName("analyticsHub")]
    public class AnalyticsHub : Hub
    {
        public static int activeUsers;
        // public IList<string> approvedUserList; 
        public void RegisterActiveUser()
        {
            activeUsers += 1;

            // Clients.OthersInGroups(approvedUserList).activeUsersUpdate(activeUsers);
            Clients.All.activeUsersUpdate(activeUsers);
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            if (stopCalled)
            {
                // Stop() called on a client, connection has been shut down.
                activeUsers -= 1;

                // Clients.OthersInGroups(approvedUsersList).activeUsersUpdate(activeUsers);
                Clients.All.activeUsersUpdate(activeUsers);
            }
            else
            {
                // Server hasn't heard from client in ~ 35 seconds
                // If you use a backend service, such as Service Bus, which supports multiple
                // servers, then the user may be connected to a different server not disconnected.
            }

            return base.OnDisconnected(stopCalled);
        }
    }
}
