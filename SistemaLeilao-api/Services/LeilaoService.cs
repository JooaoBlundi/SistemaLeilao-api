using Flunt.Notifications;
using Microsoft.AspNetCore.SignalR; // Add this for IHubContext
using Microsoft.EntityFrameworkCore;
using SistemaLeilao_api.Data;
using SistemaLeilao_api.Models;
using SistemaLeilao_api.Hubs; // Add this for AuctionHub
using SistemaLeilao_api.Interfaces;
using SistemaLeilao_api.Entities;
using System.Collections.Generic; // Required for IEnumerable
using System.Linq; // Required for LINQ methods like Where
using System.Text.Json; // For handling JSON serialization if needed for Imagens
using System.Threading.Tasks;

namespace SistemaLeilao_api.Services
{
    public class LeilaoService : Notifiable<Notification>, ILeilaoService
    {
        private readonly LeilaoDbContext _context;
        // Inject IHubContext for SignalR notifications
        private readonly IHubContext<AuctionHub> _hubContext;

        public LeilaoService(LeilaoDbContext context, IHubContext<AuctionHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext; // Assign injected context
        }

        public async Task<Leilao?> CreateLeilaoAsync(CreateLeilaoDto leilaoDto, long vendedorId)
        {
            // Validate DTO
            leilaoDto.Validate();
            if (!leilaoDto.IsValid)
            {
                AddNotifications(leilaoDto.Notifications);
                return null;
            }

            // Verify if seller exists
            var vendedor = await _context.Usuarios.FindAsync(vendedorId);
            if (vendedor == null)
            {
                AddNotification("VendedorId", "Usuário vendedor inválido.");
                return null;
            }

            var newLeilao = new Leilao
            {
                Titulo = leilaoDto.Titulo,
                Descricao = leilaoDto.Descricao,
                PrecoInicial = leilaoDto.PrecoInicial,
                // A propriedade Imagens foi removida, o gerenciamento será feito separadamente
                DataInicio = leilaoDto.DataInicio ?? DateTime.UtcNow, // Default start time if not provided
                DataFim = leilaoDto.DataFim, // End time might be null initially or set later
                Status = "Novo", // Initial status
                DataCadastro = DateTime.UtcNow,
                VendedorId = vendedorId,
                CompradorId = null, // No buyer initially
                ValorFinal = null // No final value initially
            };

            _context.Leiloes.Add(newLeilao);
            await _context.SaveChangesAsync();

            // Send notification via SignalR after saving
            await _hubContext.Clients.All.SendAsync("ReceiveNewAuction", newLeilao.Id, newLeilao.Titulo, vendedor.Nome); 

            return newLeilao;
        }

        public async Task<IEnumerable<Leilao>> GetActiveLeiloesAsync()
        {
            // Define "active" criteria (e.g., Status is "Aberto" or similar, and current time is within DataInicio and DataFim)
            // This is a basic example, refine the logic based on the exact status flow
            var now = DateTime.UtcNow;
            return await _context.Leiloes
                                 .Where(l => l.Status == "Aberto" && l.DataInicio <= now && (l.DataFim == null || l.DataFim > now))
                                 .Include(l => l.Vendedor) // Include seller info if needed
                                 .OrderByDescending(l => l.DataInicio) // Example ordering
                                 .ToListAsync();
            // Consider returning a DTO list to avoid exposing the full entity
        }

        public async Task<IEnumerable<Leilao>> GetUserLeiloesAsync(long vendedorId)
        {
            // Get all auctions created by the specified seller ID
            return await _context.Leiloes
                                 .Where(l => l.VendedorId == vendedorId)
                                 .OrderByDescending(l => l.DataCadastro) // Order by creation date
                                 .ToListAsync();
            // Consider returning a DTO list
        }

        public async Task<Leilao?> GetLeilaoByIdAsync(long leilaoId)
        {
            // Find the auction by ID, potentially including related data if needed elsewhere
            return await _context.Leiloes
                                 .Include(l => l.Vendedor) // Include seller info
                                 // .Include(l => l.ImagensLeilao) // Optionally include images if needed by caller
                                 .FirstOrDefaultAsync(l => l.Id == leilaoId);
        }

        // TODO: Implement other methods like GetUserBidsAsync (MeusLances), SearchLeiloesAsync (BuscarLances), PlaceBidAsync, etc.
        // TODO: Implement logic to change auction status (e.g., to "Aberto", "Finalizado") and send "ReceiveAuctionEnd" notification.
    }
}

