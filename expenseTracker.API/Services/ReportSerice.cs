using ClosedXML.Excel;
using ExpencseTracker.Data;
using expenseTracker.API.Models;
using Microsoft.EntityFrameworkCore;

public class ReportService : IReportService
{
    private readonly AppDbContext _context;

    public ReportService(AppDbContext context)
    {
        _context = context;
    }

   
   public async Task<byte[]> GenerateExcelReport(int userId, DateTime start, DateTime end)
{
    var transactions = await _context.Transactions
        .Include(t => t.Account)
        .Include(t => t.Subcategory)
            .ThenInclude(sc => sc.Category)
        .Where(t => t.Account!.UserId == userId &&
                    t.Date >= start &&
                    t.Date <= end)
        .ToListAsync();

    using var workbook = new XLWorkbook();
    var ws = workbook.Worksheets.Add("Spese");

    // Intestazioni
    ws.Cell(1, 1).Value = "Data";
    ws.Cell(1, 2).Value = "Importo";
    ws.Cell(1, 3).Value = "Categoria";
    ws.Cell(1, 4).Value = "Sottocategoria";
    ws.Cell(1, 5).Value = "Conto";
    ws.Cell(1, 6).Value = "Descrizione";

    for (int i = 0; i < transactions.Count; i++)
    {
        var t = transactions[i];
        ws.Cell(i + 2, 1).Value = t.Date.ToString("yyyy-MM-dd");
        ws.Cell(i + 2, 2).Value = t.Amount;
        ws.Cell(i + 2, 3).Value = t.Subcategory?.Category?.Name ?? "-";
        ws.Cell(i + 2, 4).Value = t.Subcategory?.Name ?? "-";
        ws.Cell(i + 2, 5).Value = t.Account?.Name ?? "-";
        ws.Cell(i + 2, 6).Value = t.Description ?? "";
    }

    // Riepilogo per categoria
    var categorySummary = transactions
        .GroupBy(t => t.Subcategory?.Category?.Name ?? "Sconosciuta")
        .Select(g => new
        {
            Categoria = g.Key,
            Totale = g.Sum(t => t.Amount)
        })
        .OrderBy(g => g.Categoria)
        .ToList();

    var summarySheet = workbook.Worksheets.Add("Riepilogo Categorie");
    summarySheet.Cell(1, 1).Value = "Categoria";
    summarySheet.Cell(1, 2).Value = "Totale";

    for (int i = 0; i < categorySummary.Count; i++)
    {
        summarySheet.Cell(i + 2, 1).Value = categorySummary[i].Categoria;
        summarySheet.Cell(i + 2, 2).Value = categorySummary[i].Totale;
    }

    // Saldo attuale dei conti
    var accounts = await _context.Accounts
        .Where(a => a.UserId == userId)
        .ToListAsync();

    var accountBalances = accounts.Select(a =>
    {
        var accountTransactions = transactions.Where(t => t.AccountId == a.Id);
        var saldo = a.InitialBalance + accountTransactions.Sum(t => t.Amount);
        return new
        {
            Conto = a.Name,
            Saldo = saldo
        };
    }).ToList();

    var balanceSheet = workbook.Worksheets.Add("Saldo Conti");
    balanceSheet.Cell(1, 1).Value = "Conto";
    balanceSheet.Cell(1, 2).Value = "Saldo Attuale";

    for (int i = 0; i < accountBalances.Count; i++)
    {
        balanceSheet.Cell(i + 2, 1).Value = accountBalances[i].Conto;
        balanceSheet.Cell(i + 2, 2).Value = accountBalances[i].Saldo;
    }

    // Saving Goals
    var savingGoals = await _context.SavingGoals
        .Include(sg => sg.Account)
        .Include(sg => sg.Transfers)
        .Where(sg => sg.Account!.UserId == userId)
        .ToListAsync();

    var goalsSheet = workbook.Worksheets.Add("Risparmi");

    goalsSheet.Cell(1, 1).Value = "Obiettivo";
    goalsSheet.Cell(1, 2).Value = "Conto";
    goalsSheet.Cell(1, 3).Value = "Target (€)";
    goalsSheet.Cell(1, 4).Value = "Risparmiato (€)";
    goalsSheet.Cell(1, 5).Value = "% Completamento";

    for (int i = 0; i < savingGoals.Count; i++)
    {
        var g = savingGoals[i];
        var saved = g.Transfers.Sum(t => t.Amount);
        var progress = g.TargetAmount > 0 ? (saved / g.TargetAmount) * 100 : 0;

        goalsSheet.Cell(i + 2, 1).Value = g.Name;
        goalsSheet.Cell(i + 2, 2).Value = g.Account?.Name ?? "-";
        goalsSheet.Cell(i + 2, 3).Value = g.TargetAmount;
        goalsSheet.Cell(i + 2, 4).Value = saved;
        goalsSheet.Cell(i + 2, 5).Value = $"{progress:F2}%";
    }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
  public async Task<ServiceResponse<object>> GetSummary(int userId, DateTime start, DateTime end)
    {
        var transactions = await _context.Transactions
            .Include(t => t.Subcategory)
            .ThenInclude(sc => sc.Category)
            .Include(t => t.Account)
            .Where(t => t.Account!.UserId == userId && t.Date >= start && t.Date <= end)
            .ToListAsync();

        var totalExpenses = transactions
            .Where(t => t.Amount < 0)
            .Sum(t => t.Amount);

        var totalIncome = transactions
            .Where(t => t.Amount > 0)
            .Sum(t => t.Amount);

        var byCategory = transactions
            .GroupBy(t => t.Subcategory?.Category?.Name ?? "Altro")
            .ToDictionary(
                g => g.Key,
                g => g.Sum(t => t.Amount)
            );

        var totalBalance = await _context.Accounts
            .Where(a => a.UserId == userId)
            .SumAsync(a => a.InitialBalance);

        var result = new
        {
            totalExpenses = Math.Abs(totalExpenses),
            totalIncome,
            netBalance = totalIncome + totalExpenses,
            byCategory,
            totalBalance
        };

        return new ServiceResponse<object> { Data = result };
    }


}
