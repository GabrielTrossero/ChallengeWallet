using Kata.Wallet.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Kata.Wallet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalletController : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<List<Domain.Wallet>>> GetAll()
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] WalletDto wallet)
    {
        throw new NotImplementedException();
    }
}
