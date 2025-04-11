using expenseTracker.API.Models;

public interface IAccountService
{
    Task<ServiceResponse<List<AccountResponseDto>>> GetAll(int userId);
    Task<ServiceResponse<AccountResponseDto>> Create(int userId, AccountCreateDto dto);
    Task<ServiceResponse<AccountResponseDto>> GetById(int userId, int accountId);
    Task<ServiceResponse<AccountResponseDto>> Update(int userId, int accountId, AccountCreateDto dto);
    Task<ServiceResponse<string>> Delete(int userId, int accountId);

}
