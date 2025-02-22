using AutoMapper;
using Kata.Wallet.Domain;
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

    [HttpPost("Create")]
    public async Task<ActionResult> Create([FromBody] WalletCreateDto walletDto)
    {
        var wallet = _walletService.ConvertToWallet(walletDto);
        await _walletService.Create(wallet);

        return Ok();
    }

    [HttpGet("GetAll")]
    public async Task<ActionResult<List<Domain.Wallet>>> GetAll()
    {
        var wallets = await _walletService.GetAll();

        return Ok(wallets);
    }

    [HttpGet("Filter")]
    public async Task<ActionResult<List<Domain.Wallet>>> Filter([FromQuery] WalletDto filter)
    {
        var wallet = _walletService.ConvertToWallet(filter);
        var wallets = await _walletService.Filter(wallet);

        return Ok(wallets);
    }
}
