using expenseTracker.API.Models;

public interface IBudgetService
{
    Task<ServiceResponse<List<BudgetResponseDto>>> GetAll(int userId);
    Task<ServiceResponse<BudgetResponseDto>> GetById(int id, int userId);
    Task<ServiceResponse<BudgetResponseDto>> Create(int userId, BudgetCreateDto dto);
    Task<ServiceResponse<BudgetResponseDto>> Update(int id, int userId, BudgetCreateDto dto);
    Task<ServiceResponse<string>> Delete(int id, int userId);
}

