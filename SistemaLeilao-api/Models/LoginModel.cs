using System.ComponentModel.DataAnnotations;

namespace SistemaLeilao_api.Models
{
    public class LoginModel
    {
        public string? Email { get; set; }

        public string? Senha { get; set; }

        public bool ManterConectado { get; set; } = false;
    }
}

