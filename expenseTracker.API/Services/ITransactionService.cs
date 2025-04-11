using expenseTracker.API.Models;

public interface ITransactionService
{
    Task<ServiceResponse<List<TransactionResponseDto>>> GetAll(int userId);
    Task<ServiceResponse<TransactionResponseDto>> GetById(int id, int userId);
    Task<ServiceResponse<TransactionResponseDto>> Create(int userId, TransactionCreateDto dto);
    Task<ServiceResponse<TransactionResponseDto>> Update(int id, int userId, TransactionCreateDto dto); // 👈 Aggiungi questa
    Task<ServiceResponse<string>> Delete(int id, int userId);
}
