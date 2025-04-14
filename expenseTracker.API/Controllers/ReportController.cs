using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export([FromQuery] DateTime start, [FromQuery] DateTime end)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var fileBytes = await _reportService.GenerateExcelReport(userId, start, end);
        var fileName = $"report_{start:yyyy-MM-dd}_to_{end:yyyy-MM-dd}.xlsx";

        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpGet("summary")]

    public async Task<IActionResult> GetSummary([FromQuery] DateTime start, [FromQuery] DateTime end)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var response = await _reportService.GetSummary(userId, start, end);
        return StatusCode(response.StatusCode, response);
    }
}
