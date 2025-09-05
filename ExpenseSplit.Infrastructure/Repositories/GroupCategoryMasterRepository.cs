using ExpenseSplit.Domain.Entities;
using ExpenseSplit.Domain.Interfaces;
using ExpenseSplit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplit.Infrastructure.Repositories;

public class GroupCategoryMasterRepository : IGroupCategoryMasterRepository
{
    private readonly ApplicationContext _context;

    public GroupCategoryMasterRepository(ApplicationContext context)
    {
        _context = context;
    }
    public async Task<GroupCategoryMaster> AddAsync(GroupCategoryMaster entity)
    {
        await _context.AddAsync(entity);
        return entity;
    }

    public async Task<List<GroupCategoryMaster>> GetAllAsync() => await _context.GroupCategoryMasters.ToListAsync();
    
}
