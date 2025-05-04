using Flunt.Notifications;
using Microsoft.AspNetCore.SignalR; // Add this for IHubContext
using Microsoft.EntityFrameworkCore;
using SistemaLeilao_api.Data;
using SistemaLeilao_api.Hubs; // Add this for AuctionHub
using SistemaLeilao_api.Interfaces;
using SistemaLeilao_api.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaLeilao_api.Services
{
    public class LanceService : Notifiable<Notification>, ILanceService
    {
        private readonly LeilaoDbContext _context;
        // Inject IHubContext for SignalR notifications
        private readonly IHubContext<AuctionHub> _hubContext;

        public LanceService(LeilaoDbContext context, IHubContext<AuctionHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext; // Assign injected context
        }

        public async Task<Lance?> PlaceBidAsync(long leilaoId, decimal valor, long compradorId) // Parameter name kept for consistency with controller/DTO
        {
            // 1. Validate Input (Basic validation)
            if (valor <= 0)
            {
                AddNotification("Valor", "O valor do lance deve ser positivo.");
                return null;
            }

            // 2. Find the auction and the bidder
            var leilao = await _context.Leiloes.FindAsync(leilaoId);
            var comprador = await _context.Usuarios.FindAsync(compradorId);

            if (leilao == null)
            {
                AddNotification("LeilaoId", "Leilão não encontrado.");
                return null;
            }
            if (comprador == null)
            {
                AddNotification("CompradorId", "Usuário comprador não encontrado.");
                return null;
            }

            // 3. Check Auction Status and Time
            var now = DateTime.UtcNow;
            if (leilao.Status != "Aberto" || leilao.DataInicio > now || (leilao.DataFim != null && leilao.DataFim <= now))
            {
                AddNotification("Leilao", "Leilão não está aberto para lances.");
                return null;
            }

            // 4. Check if bidder is the seller
            if (leilao.VendedorId == compradorId)
            {
                 AddNotification("UsuarioId", "O vendedor não pode dar lances no próprio leilão."); // Changed notification key
                return null;
            }

            // 5. Check if bid amount is valid (higher than current highest bid or initial price)
            var highestBid = await _context.Lances
                                         .Where(l => l.LeilaoId == leilaoId)
                                         .OrderByDescending(l => l.Valor)
                                         .FirstOrDefaultAsync();

            decimal minBidValue = highestBid?.Valor ?? leilao.PrecoInicial;

            if (valor <= minBidValue)
            {
                // Format currency for error message
                string formattedMinBid = minBidValue.ToString("C", new System.Globalization.CultureInfo("pt-BR"));
                AddNotification("Valor", $"O lance deve ser maior que {formattedMinBid}.");
                return null;
            }

            // 6. Create and Save the Bid
            var newLance = new Lance
            {
                LeilaoId = leilaoId,
                UsuarioId = compradorId, // Corrected property name
                Valor = valor,
                DataHora = now // Corrected property name
            };

            _context.Lances.Add(newLance);
            await _context.SaveChangesAsync();

            // Send notification via SignalR about the new bid
            await _hubContext.Clients.All.SendAsync("ReceiveBidUpdate", leilaoId, comprador.Nome, valor);
            // Consider sending only to a group: await _hubContext.Clients.Group($"Auction-{leilaoId}").SendAsync(...);

            return newLance;
        }

        public async Task<IEnumerable<Lance>> GetBidsByLeilaoAsync(long leilaoId)
        {
            return await _context.Lances
                                 .Where(l => l.LeilaoId == leilaoId)
                                 .Include(l => l.Usuario) // Corrected navigation property name
                                 .OrderByDescending(l => l.DataHora) // Corrected property name
                                 .ToListAsync();
            // Consider returning a DTO list
        }

        public async Task<IEnumerable<Lance>> GetBidsByUserAsync(long compradorId) // Parameter name kept for consistency
        {
             return await _context.Lances
                                 .Where(l => l.UsuarioId == compradorId) // Corrected property name
                                 .Include(l => l.Leilao) // Include auction info
                                 .OrderByDescending(l => l.DataHora) // Corrected property name
                                 .ToListAsync();
            // Consider returning a DTO list
        }

        // TODO: Implement logic to handle auction closing and send "ReceiveAuctionEnd" notification.
        // This might involve a separate method in LeilaoService or a background task.
    }
}

