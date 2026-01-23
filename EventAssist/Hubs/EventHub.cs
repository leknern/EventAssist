using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace EventAssist.Hubs
{
    [Authorize]
    public class EventHub : Hub
    {
    }
}
