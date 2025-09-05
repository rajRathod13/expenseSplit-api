using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ExpenseSplit.Domain.Entities;

public class SplitDetail : BaseClass
{
    [Key]
    public Guid SplitDetailId { get; set; }
    public Guid ExpenseId { get; set; }
    
    [JsonIgnore]
    public Expense Expense { get; set; }
    public string UserId { get; set; }
    [JsonIgnore]
    public User User { get; set; }
    [Precision(18, 2)]
    public decimal ShareAmount { get; set; }
    [Precision(5, 2)]
    public decimal? Percentage { get; set; }
    public bool IsSettled { get; set; }
}
