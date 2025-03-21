﻿using AutoMapper;
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
    private readonly IWalletMappingService _walletMappingService;

    public WalletController(IWalletService walletService, IWalletMappingService walletMappingService)
    {
        _walletService = walletService;
        _walletMappingService = walletMappingService;
    }

    [HttpPost("Create")]
    public async Task<ActionResult> Create([FromBody] WalletCreateDto walletDto)
    {
        var wallet = _walletMappingService.ConvertToWallet(walletDto);
        var errorMessage = await _walletService.Create(wallet);

        if (!string.IsNullOrEmpty(errorMessage))
        {
            return BadRequest(errorMessage);
        }

        var walletCreated = await _walletService.GetWallet(wallet.UserDocument, wallet.Currency);

        if (walletCreated == null)
        {
            return NotFound();
        }

        var walletCreatedDto = _walletMappingService.ConvertToWalletDto(walletCreated);

        return Ok(walletCreatedDto);
    }

    [HttpGet("GetAll")]
    public async Task<ActionResult<List<WalletDto>>> GetAll()
    {
        var wallets = await _walletService.GetAll();
        var walletsDto = _walletMappingService.ConvertToWalletDto(wallets);

        return Ok(walletsDto);
    }

    [HttpGet("Filter")]
    public async Task<ActionResult<List<WalletDto>>> Filter([FromQuery] string? userDocument = null, Currency? currency = null)
    {
        var wallets = await _walletService.Filter(userDocument, currency);
        var walletsDto = _walletMappingService.ConvertToWalletDto(wallets);

        return Ok(walletsDto);
    }
}
