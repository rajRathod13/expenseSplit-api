namespace ExpenseSplit.Common.RequestDTOs.Subscription;

public class RemoveUserRequest
{
    public Guid GroupId { get; set; }
    public string UserId { get; set; }
}
