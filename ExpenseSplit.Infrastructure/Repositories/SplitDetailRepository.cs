using ExpenseSplit.Domain.Entities;
using ExpenseSplit.Domain.Interfaces;
using ExpenseSplit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplit.Infrastructure.Repositories;

public class SplitDetailRepository : ISplitDetailRepository
{
    private readonly ApplicationContext _context;

    public SplitDetailRepository(ApplicationContext context)
    {
        _context = context;
    }

    public async Task AddManySplitDetailsAsync(List<SplitDetail> splits, CancellationToken cancellationToken)
    {
        await _context.SplitDetails.AddRangeAsync(splits, cancellationToken);
        await Task.CompletedTask;
    }

    public async Task DeleteByExpenseIdAsync(Guid expenseId)
    {
        var splitDetails = await _context.SplitDetails
                                         .Where(sd => sd.ExpenseId == expenseId)
                                         .ToListAsync();

        if (splitDetails.Any())
        {
            _context.SplitDetails.RemoveRange(splitDetails);
            await Task.CompletedTask;
        }
    }

    public Task<SplitDetail> AddSplitDetailAsync(SplitDetail splitDetail)
    {
        throw new NotImplementedException();
    }
}
