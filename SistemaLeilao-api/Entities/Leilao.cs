using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json; // For handling JSON column

namespace SistemaLeilao_api.Entities
{
    [Table("leiloes")] // Explicitly map to the table name from the image
    public class Leilao
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required(ErrorMessage = "O campo Título é obrigatório.")]
        [StringLength(100)]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Descrição é obrigatório.")]
        public string Descricao { get; set; } = string.Empty; 


        [Required(ErrorMessage = "O campo Preço Inicial é obrigatório.")]
        [Column(TypeName = "decimal(18, 2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço inicial deve ser maior que zero.")]
        public decimal PrecoInicial { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? ValorFinal { get; set; }

        public DateTime? DataInicio { get; set; }

        public DateTime? DataFim { get; set; }

        [StringLength(50)]
        public string? Status { get; set; } 

        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

        public long? VendedorId { get; set; }
        public long? CompradorId { get; set; }

        [ForeignKey("VendedorId")]
        public virtual Usuario? Vendedor { get; set; }

        [ForeignKey("CompradorId")]
        public virtual Usuario? Comprador { get; set; }

        public virtual ICollection<Lance> Lances { get; set; } = new List<Lance>();
        public virtual ICollection<ImagemLeilao> ImagensLeilao { get; set; } = new List<ImagemLeilao>(); // Adicionada coleção para imagens

        // Removido Helper method to get/set images as a list (not mapped to DB)
        // [NotMapped]
        // public List<string> ImagemList
        // {
        //     get => JsonSerializer.Deserialize<List<string>>(Imagens ?? "[]") ?? new List<string>();
        //     set => Imagens = JsonSerializer.Serialize(value ?? new List<string>());
        // }
    }
}

