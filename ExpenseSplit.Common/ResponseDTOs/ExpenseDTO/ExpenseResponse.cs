using ExpenseSplit.Common.ResponseDTOs.GroupDetail;
using ExpenseSplit.Common.ResponseDTOs.User;

namespace ExpenseSplit.Common.ResponseDTOs.ExpenseDTO;

public class ExpenseResponse
{
    public Guid ExpenseId { get; set; }
    public Guid GroupId { get; set; }
    public string Description { get; set; }
    public decimal TotalAmount { get; set; }
    public string SplitType { get; set; }
    public string PaidById { get; set; }
    public DateTime CreatedOn { get; set; }
    public GroupDetailResponse GroupDetail { get; set; }
    public UserResponse User{ get; set; }
    public List<SplitDetailResponse> SplitDetails { get; set; } 

}


public class SplitDetailResponse 
{
    public Guid SplitDetailId { get; set; }
    public Guid ExpenseId { get; set; }
    public string UserId { get; set; }
    public UserResponse User { get; set; }
    public decimal ShareAmount { get; set; }
    public decimal? Percentage { get; set; }
}
