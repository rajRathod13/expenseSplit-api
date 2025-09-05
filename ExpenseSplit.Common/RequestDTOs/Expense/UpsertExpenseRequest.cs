namespace ExpenseSplit.Common.RequestDTOs.Expense;

public class UpsertExpenseRequest
{
    public Guid? ExpenseId { get; set; }
    public Guid GroupId { get; set; }
    public string Description { get; set; }
    public decimal TotalAmount { get; set; }
    public string SplitType { get; set; }
    public string PaidById { get; set; }
    public List<SplitDetailDTO> SplitDetails { get; set; }
}

public class SplitDetailDTO
{
    public string UserId { get; set; }
    public decimal? Percentage { get; set; } // Only for Percentage type
    public decimal? ShareAmount { get; set; } // Only for Custom type
}
