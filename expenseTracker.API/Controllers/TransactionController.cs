using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    private int GetUserId()
    {
        return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await _transactionService.GetAll(GetUserId());
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _transactionService.GetById(id, GetUserId());
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TransactionCreateDto dto)
    {
        var response = await _transactionService.Create(GetUserId(), dto);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _transactionService.Delete(id, GetUserId());
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] TransactionCreateDto dto)
    {
        var response = await _transactionService.Update(id, GetUserId(), dto);
        return StatusCode(response.StatusCode, response);
    }
}
