using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaLeilao_api.Models
{
    [Table("lances")] // Explicitly map to the table name from the image
    public class Lance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        // Foreign Keys
        public long LeilaoId { get; set; }
        public long UsuarioId { get; set; }

        [Required(ErrorMessage = "O valor do lance é obrigatório.")]
        [Column(TypeName = "decimal(18, 2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor do lance deve ser maior que zero.")]
        public decimal Valor { get; set; }

        public DateTime DataHora { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("LeilaoId")]
        public virtual Leilao? Leilao { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }
    }
}

