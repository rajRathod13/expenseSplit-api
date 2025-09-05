using ExpenseSplit.Common.ResponseDTOs.GroupDetail;
using ExpenseSplit.Common.ResponseDTOs.User;

namespace ExpenseSplit.Common.ResponseDTOs.Invitation;

public class InvitationResponse
{
    public Guid InvitationId { get; set; }
    public Guid GroupId { get; set; }
    public GroupDetailResponse Group { get; set; }
    public string InviterId { get; set; }
    public UserResponse InviterUser { get; set; }
    public string InvitedUserId { get; set; }
    public UserResponse InvitedUser { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
}
