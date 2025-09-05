using Microsoft.AspNetCore.Http;

namespace ExpenseSplit.Common.RequestDTOs.User;

public class UpdateProfileRequest
{
    public string FullName { get; set; }
    public IFormFile ProfilePicture { get; set; }
}
