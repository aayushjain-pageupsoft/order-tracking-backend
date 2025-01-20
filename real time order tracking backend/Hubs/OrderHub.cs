using Microsoft.AspNetCore.SignalR;

public class OrderHub : Hub
{
    public async Task BroadcastOrderUpdate(string orderId, string status)
    {
        await Clients.All.SendAsync("ReceiveOrderUpdate", orderId, status);
    }
}
