using ExpenseSplit.Domain.Entities;
using ExpenseSplit.Domain.Interfaces;
using ExpenseSplit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplit.Infrastructure.Repositories;

public class ExpenseRepository : IExpenseRepository
{
    private readonly ApplicationContext _context;

    public ExpenseRepository(ApplicationContext context)
    {
        _context = context;
    }
    public async Task<Expense> AddExpenseAsync(Expense expense, CancellationToken cancellationToken)
    {
        await _context.Expenses.AddAsync(expense, cancellationToken);
        return expense;
    }

    public async Task UpdateExpenseAsync(Expense expense)
    {
        _context.Expenses.Update(expense);
        await Task.CompletedTask;
    }

    public async Task<Expense> GetExpenceByIdAsync(Guid expenseId, CancellationToken cancellationToken) 
    {
        var expense = await _context.Expenses.FirstOrDefaultAsync(x => x.ExpenseId == expenseId, cancellationToken);
        return expense;
    }

    public async Task<List<Expense>> GetExpensesByGroupIdAsync(Guid groupId, bool latestOnly, CancellationToken cancellationToken)
    {
        IQueryable<Expense> query = _context.Expenses.Where(x => x.GroupId == groupId)
                                                     .Include(x => x.SplitDetails)
                                                     .ThenInclude(x => x.User)
                                                     .Include(x => x.GroupDetail)
                                                     .Include(x => x.User)
                                                     .OrderByDescending(x => x.CreatedOn);
        if (latestOnly)
            query = query.Take(7);

        return await query.ToListAsync(cancellationToken);
    }
}
