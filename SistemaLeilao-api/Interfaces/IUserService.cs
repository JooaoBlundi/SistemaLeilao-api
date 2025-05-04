using SistemaLeilao_api.Entities;
using SistemaLeilao_web.Model;
using System.Threading.Tasks;

namespace SistemaLeilao_api.Interfaces
{
    public interface IUserService
    {
        Task<Usuario?> RegisterUserAsync(UsuarioModel registerDto);
    }
}

