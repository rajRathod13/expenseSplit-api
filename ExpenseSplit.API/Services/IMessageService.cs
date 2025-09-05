namespace ExpenseSplit.API.Services;

public interface IMessageService
{
    Task SendNotification(Guid GroupId, string GroupTitle);
}
