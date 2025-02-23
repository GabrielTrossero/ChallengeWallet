using Kata.Wallet.Domain;
using Kata.Wallet.Dtos;
using Kata.Wallet.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kata.Wallet.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IWalletService _walletService;

        public TransactionController(ITransactionService transactionService, IWalletService walletService) 
        { 
            _transactionService = transactionService;
            _walletService = walletService;
        }

        [HttpPost("Create")]
        public async Task<ActionResult> Create([FromBody] TransactionDto transactionDto, int idWalletOrigin, int idWalletDestination)
        {
            var transaction = _transactionService.ConvertToTransaction(transactionDto);

            var errorMessage = await _transactionService.Create(transaction, idWalletOrigin, idWalletDestination);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                return BadRequest(errorMessage);
            }

            return Ok();
        }

        [HttpGet("GetTransactions")]
        public async Task<ActionResult> GetTransactions([FromQuery] int idWallet)
        {
            var transactions = await _transactionService.GetTransactions(idWallet);

            var transactionsDto = _transactionService.ConvertToTransactionDto(transactions);

            return Ok(transactionsDto);
        }
    }
}
