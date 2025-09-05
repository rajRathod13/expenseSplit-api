using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ExpenseSplit.Domain.Entities;

public class Expense : BaseClass
{
    [Key]
    public Guid ExpenseId { get; set; }

    public Guid GroupId { get; set; }
    [JsonIgnore]
    public GroupDetail GroupDetail { get; set; }
    public string PaidById { get; set; }
    [JsonIgnore]
    public User User { get; set; }
    public string Description { get; set; }
    [Precision(18, 2)]
    public decimal TotalAmount { get; set; }
    public string SplitType { get; set; }
    public ICollection<SplitDetail> SplitDetails { get; set; }
}
