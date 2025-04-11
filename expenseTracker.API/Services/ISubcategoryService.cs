using expenseTracker.API.Models;

public interface ISubcategoryService
{
    Task<ServiceResponse<List<SubcategoryResponseDto>>> GetAll(int userId);
    Task<ServiceResponse<SubcategoryResponseDto>> GetById(int userId, int id);
    Task<ServiceResponse<SubcategoryResponseDto>> Create(int userId, SubcategoryCreateDto dto);
    Task<ServiceResponse<SubcategoryResponseDto>> Update(int userId, int id, SubcategoryCreateDto dto);
    Task<ServiceResponse<string>> Delete(int userId, int id);
}
