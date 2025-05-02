using System.ComponentModel.DataAnnotations;
using Flunt.Notifications;
using Flunt.Validations;

namespace SistemaLeilao_api.DTOs
{
    public class RegisterUserDto : Notifiable<Notification>
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sobrenome é obrigatório")]
        public string Sobrenome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória")]
        [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
        public string Senha { get; set; } = string.Empty;

        [Required(ErrorMessage = "CPF é obrigatório")]
        // TODO: Add CPF validation logic (e.g., using a custom attribute or Flunt rule)
        public string Cpf { get; set; } = string.Empty;

        // Optional fields from UI image
        public DateTime DataNascimento { get; set; }
        public string Sexo { get; set; }
        public string? Endereco { get; set; }
        public string? Cidade { get; set; }
        public string? Uf { get; set; }
        public string? Agencia { get; set; }
        public string? Conta { get; set; }
        public string? ChavePix { get; set; }
        public DateTime DataCadastro { get; set; }

        public void Validate()
        {
            AddNotifications(new Contract<RegisterUserDto>()
                .Requires()
                .IsNotNullOrEmpty(Nome, "Nome", "Nome é obrigatório")
                .IsNotNullOrEmpty(Sobrenome, "Sobrenome", "Sobrenome é obrigatório")
                .IsNotNullOrEmpty(Email, "Email", "Email é obrigatório")
                .IsEmail(Email, "Email", "Email inválido")
                .IsNotNullOrEmpty(Senha, "Senha", "Senha é obrigatória")
                .IsGreaterThan(Senha, 5, "Senha", "Senha deve ter no mínimo 6 caracteres")
                .IsNotNullOrEmpty(Cpf, "Cpf", "CPF é obrigatório")
                // Basic CPF format check (11 digits) - Needs more robust validation
                .IsTrue(Cpf != null && Cpf.Replace(".", "").Replace("-", "").Length == 11, "Cpf", "CPF deve ter 11 dígitos")
                // Add other Flunt validations as needed
            );
        }
    }
}

