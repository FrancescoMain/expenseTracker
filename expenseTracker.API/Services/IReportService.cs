using expenseTracker.API.Models;

public interface IReportService
{
    Task<byte[]> GenerateExcelReport(int userId, DateTime start, DateTime end);
    Task<ServiceResponse<object>> GetSummary(int userId, DateTime start, DateTime end);


    
}