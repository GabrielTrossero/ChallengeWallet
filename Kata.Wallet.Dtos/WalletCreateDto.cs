using Kata.Wallet.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kata.Wallet.Dtos
{
    public class WalletCreateDto : WalletDto
    {
        [Required(ErrorMessage = "El 'Balance' es obligatorio.")]
        [Range(0, double.MaxValue, ErrorMessage = "El balance no puede ser negativo.")]
        public new decimal Balance { get; set; }

        [Required(ErrorMessage = "El 'Documento' es obligatorio.")]
        [MinLength(8, ErrorMessage = "El 'Documento' debe tener al menos 8 caracteres.")]
        public new string? UserDocument { get; set; }

        [Required(ErrorMessage = "El 'UserName' es obligatorio.")]
        [MinLength(5, ErrorMessage = "El 'UserName' debe tener al menos 5 caracteres.")]
        public new string? UserName { get; set; }

        [Required(ErrorMessage = "La 'Moneda' es obligatoria.")]
        public new Currency? Currency { get; set; }
    }
}
