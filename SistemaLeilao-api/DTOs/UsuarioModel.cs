using System.ComponentModel.DataAnnotations;

namespace SistemaLeilao_web.DTOs
{
    public class UsuarioModel
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Campo obrigatório.")]
        public string? Nome { get; set; }

        [Required(ErrorMessage = "Campo obrigatório.")]
        public string? Sobrenome { get; set; }

        [Required(ErrorMessage = "Campo obrigatório.")]
        [EmailAddress(ErrorMessage = "O campo Email deve conter um endereço de email válido.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Campo obrigatório.")]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
        public string? Senha { get; set; }

        [Required(ErrorMessage = "Campo obrigatório.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "O CPF deve conter 11 dígitos.")]
        public string? Cpf { get; set; }

        [Required(ErrorMessage = "Campo obrigatório.")]
        public DateTime DataNascimento { get; set; }

        [Required(ErrorMessage = "Campo obrigatório.")]
        public string? Sexo { get; set; }
        public string? Endereco { get; set; }
        public string? Cidade { get; set; }
        public string? Uf { get; set; }
        public string? Agencia { get; set; }
        public string? Conta { get; set; }
        public string? ChavePix { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}
