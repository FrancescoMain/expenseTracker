
using expenseTracker.API.Models;

public interface ISavingGoalService
{
    Task<ServiceResponse<List<SavingGoalResponseDto>>> GetAll(int userId);
    Task<ServiceResponse<SavingGoalResponseDto>> Create(int userId, SavingGoalCreateDto dto);
    Task<ServiceResponse<string>> Delete(int userId, int id);
}
