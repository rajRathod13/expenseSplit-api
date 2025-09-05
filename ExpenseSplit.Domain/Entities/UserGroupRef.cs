using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ExpenseSplit.Domain.Entities;

public class UserGroupRef : BaseClass
{
    [Key]
    public Guid UserGroupId { get; set; }
    public bool IsCreator { get; set; }
    public string UserId { get; set; }
    public Guid GroupId { get; set; }
    [JsonIgnore]
    public GroupDetail GroupDetail { get; set; }
}
