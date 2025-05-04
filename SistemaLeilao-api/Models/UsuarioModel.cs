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
                .IsNotNullOrEmpty(Nome, "Nome", "Nome � obrigat�rio")
                .IsNotNullOrEmpty(Sobrenome, "Sobrenome", "Sobrenome � obrigat�rio")
                .IsNotNullOrEmpty(Email, "Email", "Email � obrigat�rio")
                .IsEmail(Email ?? "", "Email", "Email inv�lido")
                .IsNotNullOrEmpty(Senha, "Senha", "Senha � obrigat�ria")
                .IsGreaterThan(Senha?.Length ?? 0, 5, "Senha", "Senha deve ter no m�nimo 6 caracteres")
                .IsNotNullOrEmpty(Cpf, "Cpf", "CPF � obrigat�rio")
                .IsTrue(Cpf != null && Cpf.Replace(".", "").Replace("-", "").Length == 11, "Cpf", "CPF deve ter 11 d�gitos")
            );
        }

    }

}
