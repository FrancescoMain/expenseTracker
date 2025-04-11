using AutoMapper;
using ExpencseTracker.Data;
using expenseTracker.API.Models;
using Microsoft.EntityFrameworkCore;

public class AccountService : IAccountService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public AccountService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<List<AccountResponseDto>>> GetAll(int userId)
    {
        var accounts = await _context.Accounts
            .Where(a => a.UserId == userId)
            .ToListAsync();

        var dtoList = _mapper.Map<List<AccountResponseDto>>(accounts);

        return new ServiceResponse<List<AccountResponseDto>> { Data = dtoList };
    }

    public async Task<ServiceResponse<AccountResponseDto>> Create(int userId, AccountCreateDto dto)
    {
        var account = _mapper.Map<Account>(dto);
        account.UserId = userId;

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        var responseDto = _mapper.Map<AccountResponseDto>(account);
        return new ServiceResponse<AccountResponseDto> { Data = responseDto };
    }

    public async Task<ServiceResponse<AccountResponseDto>> GetById(int userId, int accountId)
    {
        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.Id == accountId && a.UserId == userId);

        if (account == null)
        {
            return new ServiceResponse<AccountResponseDto>
            {
                Success = false,
                Message = "Account non trovato",
                StatusCode = 404
            };
        }

        var dto = _mapper.Map<AccountResponseDto>(account);
        return new ServiceResponse<AccountResponseDto> { Data = dto };
    }

    public async Task<ServiceResponse<AccountResponseDto>> Update(int userId, int accountId, AccountCreateDto dto)
    {
        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.Id == accountId && a.UserId == userId);

        if (account == null)
        {
            return new ServiceResponse<AccountResponseDto>
            {
                Success = false,
                Message = "Account non trovato",
                StatusCode = 404
            };
        }

        account.Name = dto.Name;
        account.Type = dto.Type;
        account.InitialBalance = dto.InitialBalance;

        await _context.SaveChangesAsync();

        var updatedDto = _mapper.Map<AccountResponseDto>(account);
        return new ServiceResponse<AccountResponseDto> { Data = updatedDto };
    }

    public async Task<ServiceResponse<string>> Delete(int userId, int accountId)
    {
        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.Id == accountId && a.UserId == userId);

        if (account == null)
        {
            return new ServiceResponse<string>
            {
                Success = false,
                Message = "Account non trovato",
                StatusCode = 404
            };
        }

        _context.Accounts.Remove(account);
        await _context.SaveChangesAsync();

        return new ServiceResponse<string>
        {
            Data = "Account eliminato con successo"
        };
    }

   
}
