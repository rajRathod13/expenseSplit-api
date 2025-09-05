using ExpenseSplit.Common.ResponseDTOs.User;
using ExpenseSplit.Common.ResponseDTOs.UserGroup;

namespace ExpenseSplit.Common.ResponseDTOs.GroupDetail;

public class GroupDetailResponse
{
    public Guid GroupId { get; set; }
    public string Title { get; set; }
    public string GroupImage { get; set; }
    public string UserId { get; set; }
    public Guid CategoryId { get; set; }
    public bool IsCreator { get; set; }
    public DateTime CreatedOn { get; set; }
    public UserResponse User { get; set; }

    public ICollection<UserGroupResponse> UserGroups { get; set; }
}

public class GroupResponse 
{
    public List<GroupDetailResponse> CreatedGroups { get; set; }
    public List<GroupDetailResponse> GroupsAsMember { get; set; }
}
