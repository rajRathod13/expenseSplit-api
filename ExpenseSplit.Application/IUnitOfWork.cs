using ExpenseSplit.Domain.Interfaces;

namespace ExpenseSplit.Application;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    IGroupDetailRepository GroupDetailRepository { get; }
    IGroupCategoryMasterRepository GroupCategoryMasterRepository { get; }
    IUserGroupRepository UserGroupRepository { get; }
    IExpenseRepository ExpenseRepository { get; }
    ISplitDetailRepository SplitDetailRepository { get; }
    IInvitationRepository InvitationRepository { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
