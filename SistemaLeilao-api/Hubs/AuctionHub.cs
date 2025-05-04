using Microsoft.AspNetCore.SignalR;

namespace SistemaLeilao_api.Hubs
{
    public class AuctionHub : Hub
    {
        public async Task SendBidUpdate(int auctionId, string user, decimal bidAmount)
        {
            await Clients.All.SendAsync("ReceiveBidUpdate", auctionId, user, bidAmount);
        }

        public async Task SendAuctionEnd(int auctionId, string winner, decimal finalPrice)
        {
            await Clients.All.SendAsync("ReceiveAuctionEnd", auctionId, winner, finalPrice);
        }

        public async Task SendNewAuction(int auctionId, string title, string user)
        {
            await Clients.All.SendAsync("ReceiveNewAuction", auctionId, title, user);
        }

        public async Task JoinAuctionGroup(string auctionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Auction-{auctionId}");
            await Clients.Caller.SendAsync("JoinedAuction", auctionId);
        }

        public async Task LeaveAuctionGroup(string auctionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Auction-{auctionId}");
        }

        public async Task SendBidUpdateToGroup(string auctionId, string user, decimal bidAmount)
        {
            await Clients.Group($"Auction-{auctionId}").SendAsync("ReceiveBidUpdate", auctionId, user, bidAmount);
        }
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}

