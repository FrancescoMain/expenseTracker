using AutoMapper;
using ExpencseTracker.Data;
using expenseTracker.API.Models;
using Microsoft.EntityFrameworkCore;

public class SubcategoryService : ISubcategoryService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public SubcategoryService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<List<SubcategoryResponseDto>>> GetAll(int userId)
    {
        var subcategories = await _context.Subcategories
            .Include(sc => sc.Category)
            .Where(sc => sc.Category!.UserId == userId)
            .ToListAsync();

        var dtoList = _mapper.Map<List<SubcategoryResponseDto>>(subcategories);
        return new ServiceResponse<List<SubcategoryResponseDto>> { Data = dtoList };
    }

    public async Task<ServiceResponse<SubcategoryResponseDto>> GetById(int userId, int id)
    {
        var subcategory = await _context.Subcategories
            .Include(sc => sc.Category)
            .FirstOrDefaultAsync(sc => sc.Id == id && sc.Category!.UserId == userId);

        if (subcategory == null)
            return new ServiceResponse<SubcategoryResponseDto> { Success = false, Message = "Non trovata", StatusCode = 404 };

        var dto = _mapper.Map<SubcategoryResponseDto>(subcategory);
        return new ServiceResponse<SubcategoryResponseDto> { Data = dto };
    }

    public async Task<ServiceResponse<SubcategoryResponseDto>> Create(int userId, SubcategoryCreateDto dto)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == dto.CategoryId && c.UserId == userId);
        if (category == null)
            return new ServiceResponse<SubcategoryResponseDto> { Success = false, Message = "Categoria non trovata", StatusCode = 400 };

        var subcategory = _mapper.Map<Subcategory>(dto);
        _context.Subcategories.Add(subcategory);
        await _context.SaveChangesAsync();

        var response = _mapper.Map<SubcategoryResponseDto>(subcategory);
        response.CategoryName = category.Name;

        return new ServiceResponse<SubcategoryResponseDto> { Data = response };
    }

    public async Task<ServiceResponse<SubcategoryResponseDto>> Update(int userId, int id, SubcategoryCreateDto dto)
    {
        var subcategory = await _context.Subcategories
            .Include(sc => sc.Category)
            .FirstOrDefaultAsync(sc => sc.Id == id && sc.Category!.UserId == userId);

        if (subcategory == null)
            return new ServiceResponse<SubcategoryResponseDto> { Success = false, Message = "Non trovata", StatusCode = 404 };

        subcategory.Name = dto.Name;
        subcategory.CategoryId = dto.CategoryId;

        await _context.SaveChangesAsync();

        var response = _mapper.Map<SubcategoryResponseDto>(subcategory);
        return new ServiceResponse<SubcategoryResponseDto> { Data = response };
    }

    public async Task<ServiceResponse<string>> Delete(int userId, int id)
    {
        var subcategory = await _context.Subcategories
            .Include(sc => sc.Category)
            .FirstOrDefaultAsync(sc => sc.Id == id && sc.Category!.UserId == userId);

        if (subcategory == null)
            return new ServiceResponse<string> { Success = false, Message = "Non trovata", StatusCode = 404 };

        _context.Subcategories.Remove(subcategory);
        await _context.SaveChangesAsync();

        return new ServiceResponse<string> { Data = "Sottocategoria eliminata con successo" };
    }

    
}
