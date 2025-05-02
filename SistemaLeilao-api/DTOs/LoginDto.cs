using System.ComponentModel.DataAnnotations;

namespace SistemaLeilao_api.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "O campo Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de email inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Senha é obrigatório.")]
        public string Senha { get; set; } = string.Empty;
    }
}

