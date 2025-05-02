using SistemaLeilao_api.DTOs; // Assuming a CreateLanceDto will be created
using SistemaLeilao_api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaLeilao_api.Interfaces
{
    public interface ILanceService
    {
        // Method to place a new bid
        Task<Lance?> PlaceBidAsync(long leilaoId, decimal valor, long compradorId);

        // Method to get bids for a specific auction
        Task<IEnumerable<Lance>> GetBidsByLeilaoAsync(long leilaoId);

        // Method to get bids made by a specific user ("Meus Lances")
        Task<IEnumerable<Lance>> GetBidsByUserAsync(long compradorId);
    }
}

