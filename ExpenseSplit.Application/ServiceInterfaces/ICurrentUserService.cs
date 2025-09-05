namespace ExpenseSplit.Application.ServiceInterfaces;

public interface ICurrentUserService
{
    string CurrentUserId { get; }
    string Email { get; }
}
