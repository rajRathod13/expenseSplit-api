using ExpenseSplit.Domain.Entities;

namespace ExpenseSplit.Domain.Interfaces;

public interface IExpenseRepository : IDependencyMarkerRepository
{
    Task<Expense> AddExpenseAsync(Expense expense, CancellationToken cancellationToken);

    Task UpdateExpenseAsync(Expense expense);
    Task<Expense> GetExpenceByIdAsync(Guid expenseId, CancellationToken cancellationToken);
    Task<List<Expense>> GetExpensesByGroupIdAsync(Guid groupId, bool latestOnly, CancellationToken cancellationToken);
}
