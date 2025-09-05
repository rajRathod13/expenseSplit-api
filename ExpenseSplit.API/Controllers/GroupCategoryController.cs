using ExpenseSplit.Application.Commands.GroupCategory;
using ExpenseSplit.Application.Queries.GroupCategory;
using ExpenseSplit.Common.RequestDTOs.GroupCategory;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplit.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GroupCategoryController : BaseController
{
    private readonly ISender _sender;

    public GroupCategoryController(ISender sender)
    {
        _sender = sender;
    }
    [HttpGet]
    [Route("GetGroupCategories")]
    public async Task<ActionResult> GetGroupCategories()
    {
        var response = await _sender.Send(new GetGroupCategoriesQuery());
        return (response.IsSuccess && response.Data.Count > 0) ? Ok(response) : StatusCode(StatusCodes.Status204NoContent, response);
    }

    [HttpPost]
    [Route("AddGroupCategory")]
    public async Task<ActionResult> AddGroupCstegory([FromForm] AddGroupCategoryRequest request)
    {
        var response = await _sender.Send(new AddGroupCategoryCommand { AddGroupCategoryRequest = request });
        if (response.IsSuccess)
            return Ok(response);

        return StatusCode(StatusCodes.Status500InternalServerError, new { IsSuccess = response.IsSuccess, Message = response.Message });
    }
}
