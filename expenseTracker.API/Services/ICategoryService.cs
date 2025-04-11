using expenseTracker.API.Models;

public interface ICategoryService
{
    Task<ServiceResponse<List<CategoryResponseDto>>> GetAll(int userId);
    Task<ServiceResponse<CategoryResponseDto>> GetById(int userId, int id);
    Task<ServiceResponse<CategoryResponseDto>> Create(int userId, CategoryCreateDto dto);
    Task<ServiceResponse<CategoryResponseDto>> Update(int userId, int id, CategoryCreateDto dto);
    Task<ServiceResponse<string>> Delete(int userId, int id);
}
