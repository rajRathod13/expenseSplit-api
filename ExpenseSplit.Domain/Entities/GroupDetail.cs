using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ExpenseSplit.Domain.Entities;

public class GroupDetail : BaseClass
{
    [Key]
    public Guid GroupId { get; set; }
    public string Title { get; set; }
    public string GroupImage { get; set; }
    public string UserId{ get; set; }
    [JsonIgnore]
    public User User { get; set; }
    public Guid CategoryId { get; set; }
    [JsonIgnore]
    public GroupCategoryMaster GroupCategoryMaster { get; set; }
    public ICollection<UserGroupRef> UserGroupRefs { get; set; }
    [NotMapped]
    public ICollection<Expense> Expenses { get; set; }
    
    public ICollection<Invitation> Invitations { get; set; }
}
