using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SavingGoalController : ControllerBase
{
    private readonly ISavingGoalService _service;

    public SavingGoalController(ISavingGoalService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = User.GetUserId();
        var response = await _service.GetAll(userId);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SavingGoalCreateDto dto)
    {
        var userId = User.GetUserId();
        var response = await _service.Create(userId, dto);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.GetUserId();
        var response = await _service.Delete(userId, id);
        return StatusCode(response.StatusCode, response);
    }
}
