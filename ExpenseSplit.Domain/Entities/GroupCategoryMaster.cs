
using System.ComponentModel.DataAnnotations;

namespace ExpenseSplit.Domain.Entities;

public class GroupCategoryMaster : BaseClass
{
    [Key]
    public Guid CategoryId { get; set; }
    public string Title { get; set; }
    public string CategoryIcon { get; set; }
}
