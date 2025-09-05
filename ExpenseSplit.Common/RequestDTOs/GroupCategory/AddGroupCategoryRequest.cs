using Microsoft.AspNetCore.Http;

namespace ExpenseSplit.Common.RequestDTOs.GroupCategory;

public class AddGroupCategoryRequest
{
    public string Title { get; set; }
    public IFormFile CategoryIcon { get; set; }
}
