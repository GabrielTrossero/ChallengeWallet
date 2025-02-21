using AutoMapper;
using Kata.Wallet.Dtos;
using Kata.Wallet.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kata.Wallet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;

    public WalletController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Domain.Wallet>>> GetAll()
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] WalletDto walletDto)
    {
        var wallet = _walletService.ConvertToWallet(walletDto);
        await _walletService.Create(wallet);

        throw new NotImplementedException();
    }
}
