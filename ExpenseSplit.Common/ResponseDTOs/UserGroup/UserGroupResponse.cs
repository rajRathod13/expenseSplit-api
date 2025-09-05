namespace ExpenseSplit.Common.ResponseDTOs.UserGroup;

public class UserGroupResponse
{
    public Guid UserGroupId { get; set; }
    public bool IsCreator { get; set; }
    public string UserId { get; set; }
    public Guid GroupId { get; set; }
}
