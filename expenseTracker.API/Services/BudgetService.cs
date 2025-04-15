using AutoMapper;
using ExpencseTracker.Data;
using expenseTracker.API.Models;
using Microsoft.EntityFrameworkCore;



public class BudgetService : IBudgetService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public BudgetService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<List<BudgetResponseDto>>> GetAll(int userId)
    {
        var budgets = await _context.Budgets
            .Include(b => b.Category)
            .Include(b => b.Subcategory)
            .Where(b => b.UserId == userId)
            .ToListAsync();

        var transactions = await _context.Transactions
            .Include(t => t.Subcategory)
            .ThenInclude(sc => sc.Category)
            .Include(t => t.Account)
            .Where(t => t.Account!.UserId == userId)
            .ToListAsync();

        var result = new List<BudgetResponseDto>();

        foreach (var budget in budgets)
        {
            var dto = _mapper.Map<BudgetResponseDto>(budget);

            var relevantTx = transactions.Where(t =>
                t.Date >= budget.StartDate &&
                (budget.EndDate == null || t.Date <= budget.EndDate) &&
                t.Subcategory != null &&
                t.Subcategory.CategoryId == budget.CategoryId &&
                (budget.SubcategoryId == null || t.SubcategoryId == budget.SubcategoryId)
            );

            dto.Spent = relevantTx.Sum(t => t.Amount < 0 ? -t.Amount : 0);
            result.Add(dto);
        }

        return new ServiceResponse<List<BudgetResponseDto>> { Data = result };
    }

    public async Task<ServiceResponse<BudgetResponseDto>> GetById(int id, int userId)
    {
        var budget = await _context.Budgets
            .Include(b => b.Category)
            .Include(b => b.Subcategory)
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

        if (budget == null)
        {
            return new ServiceResponse<BudgetResponseDto>
            {
                Success = false,
                Message = "Budget non trovato",
                StatusCode = 404
            };
        }

        var dto = _mapper.Map<BudgetResponseDto>(budget);
        return new ServiceResponse<BudgetResponseDto> { Data = dto };
    }

    public async Task<ServiceResponse<BudgetResponseDto>> Create(int userId, BudgetCreateDto dto)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == dto.CategoryId && c.UserId == userId);
        if (category == null)
        {
            return new ServiceResponse<BudgetResponseDto>
            {
                Success = false,
                Message = "Categoria non trovata",
                StatusCode = 400
            };
        }

        var budget = _mapper.Map<Budget>(dto);
        budget.UserId = userId;

        _context.Budgets.Add(budget);
        await _context.SaveChangesAsync();

        var response = _mapper.Map<BudgetResponseDto>(budget);
        return new ServiceResponse<BudgetResponseDto> { Data = response };
    }

    public async Task<ServiceResponse<BudgetResponseDto>> Update(int id, int userId, BudgetCreateDto dto)
    {
        var budget = await _context.Budgets
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

        if (budget == null)
        {
            return new ServiceResponse<BudgetResponseDto>
            {
                Success = false,
                Message = "Budget non trovato",
                StatusCode = 404
            };
        }

        budget.CategoryId = dto.CategoryId;
        budget.SubcategoryId = dto.SubcategoryId;
        budget.Amount = dto.Amount;
        budget.StartDate = dto.StartDate;
        budget.EndDate = dto.EndDate;

        await _context.SaveChangesAsync();

        var response = _mapper.Map<BudgetResponseDto>(budget);
        return new ServiceResponse<BudgetResponseDto> { Data = response };
    }

    public async Task<ServiceResponse<string>> Delete(int id, int userId)
    {
        var budget = await _context.Budgets
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

        if (budget == null)
        {
            return new ServiceResponse<string>
            {
                Success = false,
                Message = "Budget non trovato",
                StatusCode = 404
            };
        }

        _context.Budgets.Remove(budget);
        await _context.SaveChangesAsync();

        return new ServiceResponse<string> { Data = "Budget eliminato con successo" };
    }
}
