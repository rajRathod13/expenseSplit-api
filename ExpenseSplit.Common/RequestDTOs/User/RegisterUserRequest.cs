using Microsoft.AspNetCore.Http;

namespace ExpenseSplit.Common.RequestDTOs.User;

public class RegisterUserRequest
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public IFormFile ProfilePictureImage { get; set; }
}
