using SistemaLeilao_api.Models;
using SistemaLeilao_web.DTOs;
using System.Threading.Tasks;

namespace SistemaLeilao_api.Interfaces
{
    public interface IUserService
    {
        Task<Usuario?> RegisterUserAsync(UsuarioModel registerDto);
        // Add other user-related methods if needed (e.g., GetUserById, UpdateUser)
    }
}

