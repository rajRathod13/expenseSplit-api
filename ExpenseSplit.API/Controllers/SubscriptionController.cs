using ExpenseSplit.Application.Commands.UserGroupCommands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplit.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SubscriptionController : BaseController
{
    private readonly ISender _sender;

    public SubscriptionController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("AcceptInvitation")]
    [Authorize]
    public async Task<ActionResult> AcceptInvitation([FromQuery] Guid groupId)
    {
        var response = await _sender.Send(new AddUserGroupCommand { GroupId = groupId });
        return response.IsSuccess ? Ok(response) : StatusCode(StatusCodes.Status500InternalServerError, response);
    }
}
