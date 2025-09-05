using ExpenseSplit.Domain.Entities;

namespace ExpenseSplit.Domain.Interfaces;

public interface ISplitDetailRepository : IDependencyMarkerRepository
{
    Task<SplitDetail> AddSplitDetailAsync(SplitDetail splitDetail);

    Task AddManySplitDetailsAsync(List<SplitDetail> splits, CancellationToken cancellationToken);

    Task DeleteByExpenseIdAsync(Guid expenseId);
}
