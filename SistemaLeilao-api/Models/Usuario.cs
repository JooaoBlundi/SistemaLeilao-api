using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaLeilao_api.Models
{
    [Table("usuarios")] // Explicitly map to the table name from the image
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Sobrenome é obrigatório.")]
        [StringLength(100)]
        public string Sobrenome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Email é obrigatório.")]
        [StringLength(255)]
        [EmailAddress(ErrorMessage = "Formato de email inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Senha é obrigatório.")]
        [StringLength(255)] // Store hashed password
        public string Senha { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo CPF é obrigatório.")]
        [StringLength(14)] // Consider adding CPF validation logic/attribute later
        public string Cpf { get; set; } = string.Empty;

        public DateTime? DataNascimento { get; set; }

        [StringLength(10)]
        public string? Sexo { get; set; }

        [StringLength(255)]
        public string? Endereco { get; set; }

        [StringLength(100)]
        public string? Cidade { get; set; }

        [StringLength(2)]
        public string? Uf { get; set; }

        [StringLength(10)]
        public string? Agencia { get; set; }

        [StringLength(20)]
        public string? Conta { get; set; }

        [StringLength(255)]
        public string? ChavePix { get; set; }

        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

        // Navigation properties (optional but good practice)
        [InverseProperty("Vendedor")]
        public virtual ICollection<Leilao> LeiloesVendidos { get; set; } = new List<Leilao>();

        [InverseProperty("Comprador")]
        public virtual ICollection<Leilao> LeiloesComprados { get; set; } = new List<Leilao>();

        public virtual ICollection<Lance> Lances { get; set; } = new List<Lance>();
    }
}

