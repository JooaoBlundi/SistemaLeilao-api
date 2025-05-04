using System.ComponentModel.DataAnnotations;
using Flunt.Notifications;
using Flunt.Validations;
using System;
using System.Collections.Generic;

namespace SistemaLeilao_api.Models
{
    public class CreateLeilaoDto : Notifiable<Notification>
    {
        [Required(ErrorMessage = "Título é obrigatório")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Descrição é obrigatória")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "Preço inicial é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço inicial deve ser positivo")]
        public decimal PrecoInicial { get; set; }

        [Required(ErrorMessage = "Imagens são obrigatórias")]
        [MinLength(3, ErrorMessage = "É necessário no mínimo 3 imagens")]
        public List<string> Imagens { get; set; } = new List<string>();

        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }

        public void Validate()
        {
            AddNotifications(new Contract<CreateLeilaoDto>()
                .Requires()
                .IsNotNullOrEmpty(Titulo, "Titulo", "Título é obrigatório")
                .IsNotNullOrEmpty(Descricao, "Descricao", "Descrição é obrigatória")
                .IsGreaterThan(PrecoInicial, 0, "PrecoInicial", "Preço inicial deve ser positivo")
                .IsNotNull(Imagens, "Imagens", "Lista de imagens não pode ser nula")
                .IsGreaterThan(Imagens?.Count ?? 0, 2, "Imagens", "É necessário no mínimo 3 imagens")
            );
        }
    }
}

