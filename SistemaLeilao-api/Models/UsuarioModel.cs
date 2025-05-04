using Flunt.Validations;
using SistemaLeilao_api.Models;
using System.ComponentModel.DataAnnotations;
using Flunt.Notifications;

namespace SistemaLeilao_web.Model
{
    public class UsuarioModel : Notifiable<Notification>
    {
        public long Id { get; set; }
        public string? Nome { get; set; }
        public string? Sobrenome { get; set; }
        public string? Email { get; set; }
        public string? Senha { get; set; }
        public string? Cpf { get; set; }
        public DateTime DataNascimento { get; set; }
        public string? Sexo { get; set; }
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
                .IsEmail(Email ?? "", "Email", "Email inválido")
                .IsNotNullOrEmpty(Senha, "Senha", "Senha é obrigatória")
                .IsGreaterThan(Senha?.Length ?? 0, 5, "Senha", "Senha deve ter no mínimo 6 caracteres")
                .IsNotNullOrEmpty(Cpf, "Cpf", "CPF é obrigatório")
                .IsTrue(Cpf != null && Cpf.Replace(".", "").Replace("-", "").Length == 11, "Cpf", "CPF deve ter 11 dígitos")
            );
        }

    }

}
