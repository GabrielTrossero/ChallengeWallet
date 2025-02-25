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
        private readonly ITransactionMappingService _transactionMappingService;

        public TransactionController(ITransactionService transactionService, IWalletService walletService, ITransactionMappingService transactionMappingService) 
        { 
            _transactionService = transactionService;
            _walletService = walletService;
            _transactionMappingService = transactionMappingService;
        }

        [HttpPost("Create")]
        public async Task<ActionResult> Create([FromBody] TransactionCreateDto transactionDto, int idWalletOrigin, int idWalletDestination)
        {
            var transaction = _transactionMappingService.ConvertToTransaction(transactionDto);

            var errorMessage = await _transactionService.Create(transaction, idWalletOrigin, idWalletDestination);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                return BadRequest(errorMessage);
            }

            var transactionCreated = await _transactionService.GetLastTransaction(idWalletOrigin, idWalletDestination);

            if (transactionCreated == null)
            {
                return NotFound();
            }

            var transactionCreatedDto = _transactionMappingService.ConvertToTransactionDto(transactionCreated);

            return Ok(transactionCreatedDto);
        }

        [HttpGet("GetTransactions")]
        public async Task<ActionResult> GetTransactions([FromQuery] int idWallet)
        {
            var transactions = await _transactionService.GetTransactions(idWallet);

            var transactionsDto = _transactionMappingService.ConvertToTransactionDto(transactions);

            return Ok(transactionsDto);
        }
    }
}
