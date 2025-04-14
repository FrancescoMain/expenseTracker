using AutoMapper;
using ExpencseTracker.Data;
using expenseTracker.API.Models;
using Microsoft.EntityFrameworkCore;

public class TransactionService : ITransactionService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public TransactionService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<List<TransactionResponseDto>>> GetAll(int userId, string? monthStr = null)
        {
            DateTime? monthStart = null;
            DateTime? monthEnd = null;

            if (!string.IsNullOrEmpty(monthStr) && DateTime.TryParse($"{monthStr}-01", out var parsedDate))
            {
                monthStart = new DateTime(parsedDate.Year, parsedDate.Month, 1);
                monthEnd = monthStart.Value.AddMonths(1);
            }

            var query = _context.Transactions
                .Include(t => t.Account)
                .Include(t => t.Subcategory)
                    .ThenInclude(sc => sc.Category)
                .Where(t => t.Account!.UserId == userId);

            if (monthStart.HasValue && monthEnd.HasValue)
            {
                query = query.Where(t => t.Date >= monthStart && t.Date < monthEnd);
            }

            var transactions = await query.ToListAsync();
            var dtoList = _mapper.Map<List<TransactionResponseDto>>(transactions);
            return new ServiceResponse<List<TransactionResponseDto>> { Data = dtoList };
        }

    public async Task<ServiceResponse<TransactionResponseDto>> GetById(int id, int userId)
    {
        var transaction = await _context.Transactions
            .Include(t => t.Account)
            .Include(t => t.Subcategory)
            .FirstOrDefaultAsync(t => t.Id == id && t.Account!.UserId == userId);

        if (transaction == null)
        {
            return new ServiceResponse<TransactionResponseDto>
            {
                Success = false,
                Message = "Transazione non trovata",
                StatusCode = 404
            };
        }

        var dto = _mapper.Map<TransactionResponseDto>(transaction);
        return new ServiceResponse<TransactionResponseDto> { Data = dto };
    }

    public async Task<ServiceResponse<TransactionResponseDto>> Create(int userId, TransactionCreateDto dto)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == dto.AccountId && a.UserId == userId);
        if (account == null)
        {
            return new ServiceResponse<TransactionResponseDto>
            {
                Success = false,
                Message = "Conto non trovato",
                StatusCode = 400
            };
        }

        var transaction = _mapper.Map<Transaction>(dto);
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        var dtoResponse = _mapper.Map<TransactionResponseDto>(transaction);
        return new ServiceResponse<TransactionResponseDto> { Data = dtoResponse };
    }

    public async Task<ServiceResponse<string>> Delete(int id, int userId)
    {
        var transaction = await _context.Transactions
            .Include(t => t.Account)
            .FirstOrDefaultAsync(t => t.Id == id && t.Account!.UserId == userId);

        if (transaction == null)
        {
            return new ServiceResponse<string>
            {
                Success = false,
                Message = "Transazione non trovata",
                StatusCode = 404
            };
        }

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();

        return new ServiceResponse<string> { Data = "Transazione eliminata con successo" };
    }

    public async Task<ServiceResponse<TransactionResponseDto>> Update(int id, int userId, TransactionCreateDto dto)
{
    var transaction = await _context.Transactions
        .Include(t => t.Account)
        .FirstOrDefaultAsync(t => t.Id == id && t.Account!.UserId == userId);

    if (transaction == null)
    {
        return new ServiceResponse<TransactionResponseDto>
        {
            Success = false,
            Message = "Transazione non trovata",
            StatusCode = 404
        };
    }

    // Verifica se l'account fornito è valido
    var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == dto.AccountId && a.UserId == userId);
    if (account == null)
    {
        return new ServiceResponse<TransactionResponseDto>
        {
            Success = false,
            Message = "Conto non valido",
            StatusCode = 400
        };
    }

    transaction.Amount = dto.Amount;
    transaction.Description = dto.Description;
    transaction.Date = dto.Date ?? transaction.Date; // fallback alla vecchia data
    transaction.AccountId = dto.AccountId;
    transaction.SubcategoryId = dto.SubcategoryId;

    await _context.SaveChangesAsync();

    var responseDto = _mapper.Map<TransactionResponseDto>(transaction);
    return new ServiceResponse<TransactionResponseDto> { Data = responseDto };
}
}
