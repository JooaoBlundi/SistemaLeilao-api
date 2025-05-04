using SistemaLeilao_api.Models; 
using SistemaLeilao_api.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaLeilao_api.Interfaces
{
    public interface ILanceService
    {
        Task<Lance?> PlaceBidAsync(long leilaoId, decimal valor, long compradorId);

        Task<IEnumerable<Lance>> GetBidsByLeilaoAsync(long leilaoId);

        Task<IEnumerable<Lance>> GetBidsByUserAsync(long compradorId);
    }
}

