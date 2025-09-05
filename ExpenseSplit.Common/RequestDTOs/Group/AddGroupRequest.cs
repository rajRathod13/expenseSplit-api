using Microsoft.AspNetCore.Http;

namespace ExpenseSplit.Common.RequestDTOs.Group;

public class AddGroupRequest
{
    public string Title { get; set; }
    public IFormFile GroupImageFile { get; set; }
    public Guid CategoryId { get; set; }
}
