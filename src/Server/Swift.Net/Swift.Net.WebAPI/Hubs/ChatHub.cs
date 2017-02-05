using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Swift.Net.WebAPI.Hubs
{
    [HubName("chatHub")]
    public class ChatHub : Hub
    {
    }
}
