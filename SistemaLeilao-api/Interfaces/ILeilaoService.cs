using SistemaLeilao_api.Models;
using SistemaLeilao_api.Entities;
using System.Collections.Generic; // Required for IEnumerable
using System.Threading.Tasks;

namespace SistemaLeilao_api.Interfaces
{
    public interface ILeilaoService
    {
        Task<Leilao?> CreateLeilaoAsync(CreateLeilaoDto leilaoDto, long vendedorId);
        
        Task<IEnumerable<Leilao>> GetActiveLeiloesAsync(); 

        Task<IEnumerable<Leilao>> GetUserLeiloesAsync(long vendedorId); 

        Task<Leilao?> GetLeilaoByIdAsync(long leilaoId); 

    }
}

