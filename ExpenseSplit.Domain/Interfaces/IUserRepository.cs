using ExpenseSplit.Domain.Entities;

namespace ExpenseSplit.Domain.Interfaces;

public interface IUserRepository : IDependencyMarkerRepository
{
    Task<List<User>> GetUsersByIdsAsync(List<string> userIds, CancellationToken cancellationToken);
    Task<List<User>> GetUsersByGroupAsync(Guid groupId, CancellationToken ct);

    Task<List<User>> GetUnInvitedUsersAsync(Guid groupId, CancellationToken cancellationToken);
    Task<User> RegisterUserAsync(User entity, string password);
    Task<User> FindUserbyEmailAsync(string email);
    //Task<string> LoginUserAsync(User user);
    Task<bool> ValidatePasswordAsync(User user, string password);

    Task<string> ForgotPasswordAsync(User user);

    Task<bool> ResetPasswordAsync(User user, string token, string newPassword);

    Task<bool> ChangePasswordAsync(User user, string oldPassword, string newPassword);
    Task UpdateProfileAsync(User user);
}
