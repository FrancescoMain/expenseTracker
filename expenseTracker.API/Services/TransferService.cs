using AutoMapper;
using ExpencseTracker.Data;
using expenseTracker.API.Models;
using Microsoft.EntityFrameworkCore;

public class TransferService : ITransferService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public TransferService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<TransferResponseDto>> Create(int userId, TransferCreateDto dto)
    {
        // FromAccount obbligatorio
        var fromAccount = await _context.Accounts
            .FirstOrDefaultAsync(a => a.Id == dto.FromAccountId && a.UserId == userId);

        if (fromAccount == null)
        {
            return new ServiceResponse<TransferResponseDto>
            {
                Success = false,
                Message = "Conto di origine non trovato",
                StatusCode = 404
            };
        }

        // Casistica: Salvataggio su SavingGoal
        if (dto.SavingGoalId.HasValue)
        {
            var goal = await _context.SavingGoals
                .Include(g => g.Account)
                .FirstOrDefaultAsync(g => g.Id == dto.SavingGoalId && g.Account!.UserId == userId);

            if (goal == null)
            {
                return new ServiceResponse<TransferResponseDto>
                {
                    Success = false,
                    Message = "Obiettivo di risparmio non trovato",
                    StatusCode = 404
                };
            }

            var transfer = new Transfer
            {
                Amount = dto.Amount,
                Date = dto.Date ?? DateTime.UtcNow,
                FromAccountId = dto.FromAccountId,
                SavingGoalId = dto.SavingGoalId,
                Note = dto.Note
            };

            var debit = new Transaction
            {
                AccountId = dto.FromAccountId,
                Amount = -dto.Amount,
                Date = transfer.Date,
                Description = $"Trasferimento a obiettivo: {goal.Name}",
                IsTransfer = true
            };

            _context.Transactions.Add(debit);
            _context.Transfers.Add(transfer);

            await _context.SaveChangesAsync();

            var full = await _context.Transfers
                .Include(t => t.FromAccount)
                .Include(t => t.SavingGoal)
                .FirstAsync(t => t.Id == transfer.Id);

            var dtoResponse = _mapper.Map<TransferResponseDto>(full);
            return new ServiceResponse<TransferResponseDto> { Data = dtoResponse };
        }

        // Casistica: Trasferimento classico Account ➜ Account
        if (!dto.ToAccountId.HasValue || dto.FromAccountId == dto.ToAccountId)
        {
            return new ServiceResponse<TransferResponseDto>
            {
                Success = false,
                Message = "Conto di destinazione non valido o uguale a quello di origine",
                StatusCode = 400
            };
        }

        var toAccount = await _context.Accounts
            .FirstOrDefaultAsync(a => a.Id == dto.ToAccountId && a.UserId == userId);

        if (toAccount == null)
        {
            return new ServiceResponse<TransferResponseDto>
            {
                Success = false,
                Message = "Conto di destinazione non trovato",
                StatusCode = 404
            };
        }

        var classicTransfer = new Transfer
        {
            Amount = dto.Amount,
            Date = dto.Date ?? DateTime.UtcNow,
            FromAccountId = dto.FromAccountId,
            ToAccountId = dto.ToAccountId.Value,
            Note = dto.Note
        };

        var debitClassic = new Transaction
        {
            AccountId = dto.FromAccountId,
            Amount = -dto.Amount,
            Date = classicTransfer.Date,
            Description = $"Trasferimento verso {toAccount.Name}: {dto.Note}",
            IsTransfer = true
        };

        var creditClassic = new Transaction
        {
            AccountId = dto.ToAccountId.Value,
            Amount = dto.Amount,
            Date = classicTransfer.Date,
            Description = $"Trasferimento da {fromAccount.Name}: {dto.Note}",
            IsTransfer = true
        };

        _context.Transactions.AddRange(debitClassic, creditClassic);
        _context.Transfers.Add(classicTransfer);

        await _context.SaveChangesAsync();

        var fullClassic = await _context.Transfers
            .Include(t => t.FromAccount)
            .Include(t => t.ToAccount)
            .FirstAsync(t => t.Id == classicTransfer.Id);

        var response = _mapper.Map<TransferResponseDto>(fullClassic);
        return new ServiceResponse<TransferResponseDto> { Data = response };
    }

    public async Task<ServiceResponse<List<TransferResponseDto>>> GetAll(int userId)
    {
        var transfers = await _context.Transfers
            .Include(t => t.FromAccount)
            .Include(t => t.ToAccount)
            .Include(t => t.SavingGoal)
            .Where(t => t.FromAccount!.UserId == userId)
            .ToListAsync();

        var dtoList = _mapper.Map<List<TransferResponseDto>>(transfers);
        return new ServiceResponse<List<TransferResponseDto>> { Data = dtoList };
    }

    public async Task<ServiceResponse<string>> Delete(int id, int userId)
    {
        var transfer = await _context.Transfers
            .Include(t => t.FromAccount)
            .Include(t => t.ToAccount)
            .Include(t => t.SavingGoal)
            .FirstOrDefaultAsync(t =>
                t.Id == id &&
                (t.FromAccountId == _context.Accounts
                    .Where(a => a.UserId == userId)
                    .Select(a => a.Id)
                    .FirstOrDefault() ||
                t.ToAccountId == _context.Accounts
                    .Where(a => a.UserId == userId)
                    .Select(a => a.Id)
                    .FirstOrDefault()));

        if (transfer == null)
        {
            return new ServiceResponse<string>
            {
                Success = false,
                Message = "Trasferimento non trovato",
                StatusCode = 404
            };
        }

        // Rimuovi transazioni collegate (match su descrizione e data)
        var transactions = await _context.Transactions
            .Where(t =>
                t.IsTransfer &&
                t.Date == transfer.Date &&
                t.Amount == transfer.Amount ||
                t.Amount == -transfer.Amount
            )
            .ToListAsync();

        _context.Transactions.RemoveRange(transactions);
        _context.Transfers.Remove(transfer);

        await _context.SaveChangesAsync();

        return new ServiceResponse<string> { Data = "Trasferimento eliminato con successo" };
    }


}
