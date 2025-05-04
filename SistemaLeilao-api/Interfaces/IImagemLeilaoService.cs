using Flunt.Notifications;
using SistemaLeilao_api.Entities;

namespace SistemaLeilao_api.Interfaces
{
    public interface IImagemLeilaoService 
    {
        Task<(bool Success, List<ImagemLeilao>? Data, List<Notification> Errors)> AddImagensAsync(long leilaoId, List<IFormFile> files);
        Task<IEnumerable<ImagemLeilao>> GetImagensByLeilaoIdAsync(long leilaoId);
        Task<ImagemLeilao?> GetImagemByIdAsync(long imagemId);
        Task<bool> SetImagemPrincipalAsync(long leilaoId, long imagemId);
        Task<bool> DeleteImagemAsync(long imagemId);
    }
}

