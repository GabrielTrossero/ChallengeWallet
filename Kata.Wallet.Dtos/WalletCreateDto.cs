using Kata.Wallet.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Resources;
using Kata.Wallet.Dtos.Resources;

namespace Kata.Wallet.Dtos
{
    public class WalletCreateDto : WalletDto
    {
        private static readonly ResourceManager _resourceManager = new ResourceManager(
            "Kata.Wallet.Dtos.Resources.MessagesDtos", typeof(WalletCreateDto).Assembly);

        [Required(ErrorMessageResourceName = nameof(MessagesDtos.Required_Balance), ErrorMessageResourceType = typeof(MessagesDtos))]
        [Range(0, double.MaxValue, ErrorMessageResourceName = nameof(MessagesDtos.Range_Balance), ErrorMessageResourceType = typeof(MessagesDtos))]
        public new decimal Balance { get; set; }

        [Required(ErrorMessageResourceName = nameof(MessagesDtos.Required_Document), ErrorMessageResourceType = typeof(MessagesDtos))]
        [MinLength(8, ErrorMessageResourceName = nameof(MessagesDtos.MinLength_Document), ErrorMessageResourceType = typeof(MessagesDtos))]
        [RegularExpression(@"^\d+$", ErrorMessageResourceName = nameof(MessagesDtos.OnlyNumbers_Document), ErrorMessageResourceType = typeof(MessagesDtos))]
        public new string? UserDocument { get; set; }

        [Required(ErrorMessageResourceName = nameof(MessagesDtos.Required_UserName), ErrorMessageResourceType = typeof(MessagesDtos))]
        [MinLength(5, ErrorMessageResourceName = nameof(MessagesDtos.MinLength_UserName), ErrorMessageResourceType = typeof(MessagesDtos))]
        [MaxLength(20, ErrorMessageResourceName = nameof(MessagesDtos.MaxLength_UserName), ErrorMessageResourceType = typeof(MessagesDtos))]
        public new string? UserName { get; set; }

        [Required(ErrorMessageResourceName = nameof(MessagesDtos.Required_Currency), ErrorMessageResourceType = typeof(MessagesDtos))]
        public new Currency? Currency { get; set; }
    }
}
