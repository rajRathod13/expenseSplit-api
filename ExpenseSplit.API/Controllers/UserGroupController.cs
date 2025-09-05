using ExpenseSplit.Application.Commands.Subscription;
using ExpenseSplit.Application.Commands.UserGroupCommands;
using ExpenseSplit.Common.RequestDTOs.Subscription;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplit.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserGroupController : BaseController
{
    private readonly ISender _sender;

    public UserGroupController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("InsertUser")]
    [Authorize]
    public async Task<ActionResult> InsertUserInGroup([FromQuery] Guid groupId) 
    {
        var response = await _sender.Send(new AddUserGroupCommand { GroupId = groupId });
        return response.IsSuccess ? Ok(response) : StatusCode(StatusCodes.Status500InternalServerError, response);
    }

    [HttpPost("RemoveUserFromGroup")]
    [Authorize]
    public async Task<ActionResult> RemoveUserFromGroup([FromBody] RemoveUserRequest request)
    {
        if (request is null)
            return BadRequest();

        var response = await _sender.Send(new RemoveUserFromGropCommand { RemoveUserRequest = request });
        return response.IsSuccess ? Ok(response) : StatusCode(StatusCodes.Status500InternalServerError, response);
    }
}
