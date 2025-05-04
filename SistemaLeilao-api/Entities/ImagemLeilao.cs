using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaLeilao_api.Entities
{
    public class ImagemLeilao
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long LeilaoId { get; set; }

        [ForeignKey("LeilaoId")]
        public virtual Leilao Leilao { get; set; } = null!; 

        [Required]
        public byte[] DadosImagem { get; set; } = Array.Empty<byte>(); 

        public string? ContentType { get; set; } 

        public string? NomeArquivo { get; set; } 

        [Required]
        public bool IsPrincipal { get; set; } = false; 

        public DateTime DataUpload { get; set; } = DateTime.UtcNow;
    }
}

