using ExpenseSplit.Application;
using ExpenseSplit.Domain.Interfaces;
using ExpenseSplit.Infrastructure.Data;

namespace ExpenseSplit.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationContext _context;
    public IUserRepository UserRepository { get; }
    public IGroupDetailRepository GroupDetailRepository { get; }

    public IGroupCategoryMasterRepository GroupCategoryMasterRepository { get; }
    public IUserGroupRepository UserGroupRepository { get; }

    public IExpenseRepository ExpenseRepository { get; }
    public ISplitDetailRepository SplitDetailRepository { get; }
    public IInvitationRepository InvitationRepository { get; }

    public UnitOfWork(ApplicationContext context,
        IUserRepository userRepository,
        IGroupDetailRepository groupDetailRepository,
        IGroupCategoryMasterRepository groupCategoryMasterRepository,
        IUserGroupRepository userGroupRepository,
        IExpenseRepository expenseRepository,
        ISplitDetailRepository splitDetailRepository,
        IInvitationRepository invitationRepository)
    {
        _context = context;
        UserRepository = userRepository;
        GroupDetailRepository = groupDetailRepository;
        GroupCategoryMasterRepository = groupCategoryMasterRepository;
        UserGroupRepository = userGroupRepository;
        ExpenseRepository = expenseRepository;
        SplitDetailRepository = splitDetailRepository;
        InvitationRepository = invitationRepository;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
