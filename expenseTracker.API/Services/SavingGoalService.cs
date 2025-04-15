using AutoMapper;
using ClosedXML.Excel;
using ExpencseTracker.Data;
using expenseTracker.API.Models;
using Microsoft.EntityFrameworkCore;

public class SavingGoalService : ISavingGoalService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public SavingGoalService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<List<SavingGoalResponseDto>>> GetAll(int userId)
    {
        var goals = await _context.SavingGoals
            .Include(g => g.Account)
            .Include(g => g.Transfers)
            .Where(g => g.Account!.UserId == userId)
            .ToListAsync();

        var dtoList = _mapper.Map<List<SavingGoalResponseDto>>(goals);
        return new ServiceResponse<List<SavingGoalResponseDto>> { Data = dtoList };
    }

    public async Task<ServiceResponse<SavingGoalResponseDto>> Create(int userId, SavingGoalCreateDto dto)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == dto.AccountId && a.UserId == userId);
        if (account == null)
        {
            return new ServiceResponse<SavingGoalResponseDto> { Success = false, Message = "Conto non trovato", StatusCode = 404 };
        }

        var goal = _mapper.Map<SavingGoal>(dto);
        _context.SavingGoals.Add(goal);
        await _context.SaveChangesAsync();

        var response = _mapper.Map<SavingGoalResponseDto>(goal);
        return new ServiceResponse<SavingGoalResponseDto> { Data = response };
    }

    public async Task<ServiceResponse<string>> Delete(int userId, int id)
    {
        var goal = await _context.SavingGoals
            .Include(g => g.Account)
            .FirstOrDefaultAsync(g => g.Id == id && g.Account!.UserId == userId);

        if (goal == null)
            return new ServiceResponse<string> { Success = false, Message = "Obiettivo non trovato", StatusCode = 404 };

        _context.SavingGoals.Remove(goal);
        await _context.SaveChangesAsync();
        return new ServiceResponse<string> { Data = "Obiettivo eliminato" };
    }
}
