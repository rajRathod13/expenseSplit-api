using ExpenseSplit.Domain.Entities;
using ExpenseSplit.Domain.Enums;
using ExpenseSplit.Domain.Interfaces;
using ExpenseSplit.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ExpenseSplit.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationContext _context;
    private readonly UserManager<User> _userManager;

    public UserRepository(ApplicationContext context,
       UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<List<User>> GetUsersByIdsAsync(List<string> userIds, CancellationToken cancellationToken)
    {
        var users = await _context.Users.Where(user => userIds.Contains(user.Id)).ToListAsync(cancellationToken);
        return users;
    }

    public async Task<List<User>> GetUsersByGroupAsync(Guid groupId, CancellationToken ct)
    {
        return await (from ug in _context.UserGroups.AsNoTracking()
                      join u in _context.Users.AsNoTracking() on ug.UserId equals u.Id
                      where ug.GroupId == groupId
                      orderby ug.IsCreator descending, u.FullName // tie-breaker optional
                      select u)
                     .ToListAsync(ct);
    }

    public async Task<bool> ChangePasswordAsync(User user, string oldPassword, string newPassword)
    {
        var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        return result.Succeeded;
    }

    public async Task<User> FindUserbyEmailAsync(string email) => await _userManager.FindByEmailAsync(email);

    public async Task<string> ForgotPasswordAsync(User user) =>
        await _userManager.GeneratePasswordResetTokenAsync(user);

    //public async Task<string> LoginUserAsync(User user)
    //{
    //    try
    //    {
    //        //var roles = await _userManager.GetRolesAsync(existingUser);
    //        var roles = new List<string>();
    //        var token = _tokenService.GenerateToken(user.Email, user.Id, roles.ToList());
    //        if (string.IsNullOrWhiteSpace(token))
    //            return string.Empty;

    //        return token;
    //    }
    //    catch (Exception)
    //    {
    //        throw;
    //    }
    //}

    public async Task<User> RegisterUserAsync(User entity, string password)
    {
        try
        {
            var temp = await _userManager.CreateAsync(entity, password);
            if (temp.Succeeded)
            {
                //await _userManager.AddToRoleAsync(entity, "User");
                return entity;
            }

            return null;
        }
        catch (ArgumentNullException)
        {
            throw;
        }
        catch (DBConcurrencyException)
        {
            throw;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> ResetPasswordAsync(User user, string token, string newPassword)
    {
        var isValidToken = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "ForgotPassword", token);
        if (!isValidToken)
            throw new Exception("invalid token.");

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        return result.Succeeded;
    }

    public async Task<bool> ValidatePasswordAsync(User user, string password) =>
        await _userManager.CheckPasswordAsync(user, password);

    public async Task UpdateProfileAsync(User user)
    {
        _context.Users.Update(user);
        await Task.CompletedTask;
    }

    //public async Task<List<User>> GetUnInvitedUsersAsync(Guid groupId, CancellationToken cancellationToken)
    //{
    //    var users = await _context.Users
    //                                    .AsNoTracking()
    //                                    .Where(u => !_context.UserGroups
    //                                                .Any(ug => ug.GroupId == groupId && ug.Id == u.Id))
    //                                    .ToListAsync(cancellationToken);

    //    return users;
    //}

    public async Task<List<User>> GetUnInvitedUsersAsync(Guid groupId, CancellationToken cancellationToken)
    {
        var availableUsers = await _context.Users.AsNoTracking()
                                                .Where(u =>
                                                    !_context.UserGroups
                                                    .Any(m => m.GroupId == groupId && m.UserId == u.Id) &&   // not a member
                                                    !_context.Invitations
                                                    .Any(inv => inv.GroupId == groupId
                                                                && inv.InvitedUserId == u.Id
                                                                && inv.Status == InvitationStatus.Pending))
                                                .ToListAsync(cancellationToken);

        return availableUsers;
    }
}
