namespace ExpenseSplit.Common.RequestDTOs.Subscription;

public class InvitationRequest
{
    public Guid GroupId { get; set; }
    public string InvitedUserId { get; set; }
}
