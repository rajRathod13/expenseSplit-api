
using Microsoft.AspNetCore.Identity;

namespace ExpenseSplit.Domain.Entities;

public class User : IdentityUser, IBaseClass
{
    public string FullName { get; set; }
    public string ProfilePicture { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid UpdatedBy { get; set; }
    public DateTime UpdatedOn { get; set; }
    public ICollection<GroupDetail> GroupDetails { get; set; }
    public ICollection<SplitDetail> SplitDetails { get; set; }
}
