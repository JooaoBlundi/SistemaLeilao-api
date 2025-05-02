using Microsoft.AspNetCore.SignalR;

namespace SistemaLeilao_api.Hubs
{
    // Hub para notificações em tempo real relacionadas aos leilões
    public class AuctionHub : Hub
    {
        // Método que pode ser chamado pelo servidor para enviar uma notificação de novo lance para todos os clientes conectados
        public async Task SendBidUpdate(int auctionId, string user, decimal bidAmount)
        {
            // Envia a mensagem para todos os clientes conectados
            // Clientes podem escutar por "ReceiveBidUpdate"
            await Clients.All.SendAsync("ReceiveBidUpdate", auctionId, user, bidAmount);
        }

        // Método que pode ser chamado pelo servidor para notificar sobre o fim de um leilão
        public async Task SendAuctionEnd(int auctionId, string winner, decimal finalPrice)
        {
            await Clients.All.SendAsync("ReceiveAuctionEnd", auctionId, winner, finalPrice);
        }

        // Adicionar usuários a grupos específicos de leilão (opcional, mas útil para escalar)
        public async Task JoinAuctionGroup(string auctionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Auction-{auctionId}");
            // Poderia enviar uma mensagem de confirmação ou estado atual do leilão apenas para quem entrou
            // await Clients.Caller.SendAsync("JoinedAuction", auctionId);
        }

        public async Task LeaveAuctionGroup(string auctionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Auction-{auctionId}");
        }

        // Exemplo de como enviar mensagem apenas para um grupo específico
        public async Task SendBidUpdateToGroup(string auctionId, string user, decimal bidAmount)
        {
            await Clients.Group($"Auction-{auctionId}").SendAsync("ReceiveBidUpdate", auctionId, user, bidAmount);
        }

        // Sobrescrever métodos OnConnectedAsync/OnDisconnectedAsync se precisar de lógica específica
        // quando um cliente conecta ou desconecta.
        // public override async Task OnConnectedAsync()
        // {
        //     await base.OnConnectedAsync();
        // }

        // public override async Task OnDisconnectedAsync(Exception? exception)
        // {
        //     await base.OnDisconnectedAsync(exception);
        // }
    }
}

