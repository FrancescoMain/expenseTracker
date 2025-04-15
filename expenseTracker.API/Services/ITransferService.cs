
using expenseTracker.API.Models;

public interface ITransferService
{
    Task<ServiceResponse<TransferResponseDto>> Create(int userId, TransferCreateDto dto);
    Task<ServiceResponse<List<TransferResponseDto>>> GetAll(int userId);

    Task<ServiceResponse<string>> Delete(int id, int userId);

}
