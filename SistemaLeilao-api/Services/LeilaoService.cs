using Flunt.Notifications;
using Microsoft.EntityFrameworkCore;
using SistemaLeilao_api.Data;
using SistemaLeilao_api.DTOs;
using SistemaLeilao_api.Interfaces;
using SistemaLeilao_api.Models;
using System.Collections.Generic; // Required for IEnumerable
using System.Linq; // Required for LINQ methods like Where
using System.Text.Json; // For handling JSON serialization if needed for Imagens
using System.Threading.Tasks;

namespace SistemaLeilao_api.Services
{
    public class LeilaoService : Notifiable<Notification>, ILeilaoService
    {
        private readonly LeilaoDbContext _context;
        // Inject IHubContext if notifications are needed upon creation
        // private readonly Microsoft.AspNetCore.SignalR.IHubContext<Hubs.AuctionHub> _hubContext;

        public LeilaoService(LeilaoDbContext context /*, IHubContext<Hubs.AuctionHub> hubContext */)
        {
            _context = context;
            // _hubContext = hubContext;
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

            // Verify if seller exists (optional, but good practice)
            var vendedorExists = await _context.Usuarios.AnyAsync(u => u.Id == vendedorId);
            if (!vendedorExists)
            {
                AddNotification("VendedorId", "Usuário vendedor inválido.");
                return null;
            }

            var newLeilao = new Leilao
            {
                Titulo = leilaoDto.Titulo,
                Descricao = leilaoDto.Descricao,
                PrecoInicial = leilaoDto.PrecoInicial,
                // Store image URLs/paths as JSON string - Requires handling file upload separately
                Imagens = "[]", // Placeholder - Image handling needs separate implementation
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

            // Optional: Send notification via SignalR
            // await _hubContext.Clients.All.SendAsync("NewAuction", newLeilao); 

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

        // TODO: Implement other methods like GetLeilaoByIdAsync, GetUserBidsAsync (MeusLances), SearchLeiloesAsync (BuscarLances), PlaceBidAsync, etc.
    }
}

