using SistemaLeilao_api.Models;
using SistemaLeilao_api.Entities;

namespace SistemaLeilao_api.Interfaces
{
    public interface IAuthService
    {
        Task<string?> AuthenticateAsync(LoginModel loginDto);
    }
}

