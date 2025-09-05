namespace ExpenseSplit.Application.ServiceInterfaces;

public interface ITokenService : IDependencyMarkerService
{
    string GenerateToken(string email, string userId, List<string> roles);
    //string GetEmailFromToken(string token);
}
