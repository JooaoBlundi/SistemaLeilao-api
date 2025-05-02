using SistemaLeilao_api.DTOs;
using SistemaLeilao_api.Models;

namespace SistemaLeilao_api.Interfaces
{
    public interface IAuthService
    {
        Task<string?> AuthenticateAsync(LoginDto loginDto);
        // We might add methods for password hashing/verification if not handled elsewhere
    }
}

