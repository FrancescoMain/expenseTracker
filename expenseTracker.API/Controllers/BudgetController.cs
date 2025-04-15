using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BudgetController : ControllerBase
{
    private readonly IBudgetService _service;

    public BudgetController(IBudgetService service)
    {
        _service = service;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var res = await _service.GetAll(GetUserId());
        return StatusCode(res.StatusCode, res);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var res = await _service.GetById(id, GetUserId());
        return StatusCode(res.StatusCode, res);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BudgetCreateDto dto)
    {
        var res = await _service.Create(GetUserId(), dto);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] BudgetCreateDto dto)
    {
        var res = await _service.Update(id, GetUserId(), dto);
        return StatusCode(res.StatusCode, res);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var res = await _service.Delete(id, GetUserId());
        return StatusCode(res.StatusCode, res);
    }
}
