using ExpenseSplit.Domain.Entities;

namespace ExpenseSplit.Domain.Interfaces;

public interface IGroupCategoryMasterRepository : IDependencyMarkerRepository
{
    Task<GroupCategoryMaster> AddAsync(GroupCategoryMaster entity);
    Task<List<GroupCategoryMaster>> GetAllAsync();
}
