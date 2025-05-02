using SistemaLeilao_api.DTOs;
using SistemaLeilao_api.Models;
using System.Collections.Generic; // Required for IEnumerable
using System.Threading.Tasks;

namespace SistemaLeilao_api.Interfaces
{
    public interface ILeilaoService
    {
        // Takes the DTO and the ID of the authenticated user (seller)
        Task<Leilao?> CreateLeilaoAsync(CreateLeilaoDto leilaoDto, long vendedorId);
        
        // Method to get active auctions (e.g., for the main dashboard)
        Task<IEnumerable<Leilao>> GetActiveLeiloesAsync(); // Consider returning a LeilaoDto list

        // Method to get auctions created by a specific user
        Task<IEnumerable<Leilao>> GetUserLeiloesAsync(long vendedorId); // Consider returning a LeilaoDto list

        // Add other auction-related methods if needed (e.g., GetLeilaoById, SearchLeiloes, UpdateLeilaoStatus)
    }
}

