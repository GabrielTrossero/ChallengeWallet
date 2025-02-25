using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using Kata.Wallet.Dtos.Resources;

namespace Kata.Wallet.Dtos
{
    public class TransactionCreateDto : TransactionDto
    {
        private static readonly ResourceManager _resourceManager = new ResourceManager(
            "Kata.Wallet.Dtos.Resources.MessagesDtos", typeof(WalletCreateDto).Assembly);

        [Required(ErrorMessageResourceName = nameof(MessagesDtos.Required_Amount), ErrorMessageResourceType = typeof(MessagesDtos))]
        [Range(0, double.MaxValue, ErrorMessageResourceName = nameof(MessagesDtos.Range_Amount), ErrorMessageResourceType = typeof(MessagesDtos))]
        public new decimal Amount { get; set; }

        [MaxLength(255, ErrorMessageResourceName = nameof(MessagesDtos.MaxLength_Description), ErrorMessageResourceType = typeof(MessagesDtos))]
        public new string? Description { get; set; }
    }
}
