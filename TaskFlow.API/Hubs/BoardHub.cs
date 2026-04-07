using Microsoft.AspNetCore.SignalR;

namespace TaskFlow.API.Hubs;

public class BoardHub : Hub
{
    public async Task BroadcastTaskMoved(Guid taskId, string newStatus)
    {
        await Clients.All.SendAsync("TaskMoved", taskId, newStatus);
    }

    public async Task BroadcastTaskUpdated(Guid taskId)
    {
        await Clients.All.SendAsync("TaskUpdated", taskId);
    }
}