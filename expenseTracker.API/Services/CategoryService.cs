using AutoMapper;
using ExpencseTracker.Data;
using expenseTracker.API.Models;
using Microsoft.EntityFrameworkCore;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CategoryService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<List<CategoryResponseDto>>> GetAll(int userId)
    {
        var categories = await _context.Categories
            .Where(c => c.UserId == userId)
            .ToListAsync();

        var dtoList = _mapper.Map<List<CategoryResponseDto>>(categories);
        return new ServiceResponse<List<CategoryResponseDto>> { Data = dtoList };
    }

    public async Task<ServiceResponse<CategoryResponseDto>> GetById(int userId, int id)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (category == null)
            return new ServiceResponse<CategoryResponseDto>
            {
                Success = false,
                Message = "Categoria non trovata",
                StatusCode = 404
            };

        var dto = _mapper.Map<CategoryResponseDto>(category);
        return new ServiceResponse<CategoryResponseDto> { Data = dto };
    }

    public async Task<ServiceResponse<CategoryResponseDto>> Create(int userId, CategoryCreateDto dto)
    {
        var category = _mapper.Map<Category>(dto);
        category.UserId = userId;

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        var responseDto = _mapper.Map<CategoryResponseDto>(category);
        return new ServiceResponse<CategoryResponseDto> { Data = responseDto };
    }

    public async Task<ServiceResponse<CategoryResponseDto>> Update(int userId, int id, CategoryCreateDto dto)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (category == null)
            return new ServiceResponse<CategoryResponseDto>
            {
                Success = false,
                Message = "Categoria non trovata",
                StatusCode = 404
            };

        category.Name = dto.Name;
        await _context.SaveChangesAsync();

        var dtoOut = _mapper.Map<CategoryResponseDto>(category);
        return new ServiceResponse<CategoryResponseDto> { Data = dtoOut };
    }

    public async Task<ServiceResponse<string>> Delete(int userId, int id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (category == null)
            return new ServiceResponse<string>
            {
                Success = false,
                Message = "Categoria non trovata",
                StatusCode = 404
            };

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return new ServiceResponse<string> { Data = "Categoria eliminata con successo" };
    }
}
