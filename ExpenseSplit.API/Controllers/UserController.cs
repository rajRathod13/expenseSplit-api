using ExpenseSplit.Application.Commands.UserCommands;
using ExpenseSplit.Application.Queries.UserQuerirs;
using ExpenseSplit.Common.RequestDTOs.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplit.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : BaseController
{
    private readonly ISender _sender;

    public UserController(ISender sender)
    {
        _sender = sender;
    }
    [HttpGet("GetUserByEmail")]
    [Authorize]
    public async Task<ActionResult> GetUserByEmail()
    {
        var response = await _sender.Send(new GetUserByEmailQuery());
        return response.IsSuccess ? Ok(response) : StatusCode(StatusCodes.Status500InternalServerError, response);
    }

    [HttpPut("UpdateProfile")]
    public async Task<ActionResult> UpdateProfile([FromForm] UpdateProfileRequest request)
    {
        var response = await _sender.Send(new UpdateProfileCommand { UpdateProfileRequest = request });
        return response.IsSuccess ? Ok(response) : StatusCode(StatusCodes.Status500InternalServerError, response);
    }

    [HttpGet("GetUnInvitedUsers")]
    [Authorize]
    public async Task<ActionResult> GetUnInvitedUsers([FromQuery] Guid groupId)
    {
        var response = await _sender.Send(new GetUninvitedUsersQuery { GroupId = groupId });
        return (response.IsSuccess && response.Data.Count > 0) ? Ok(response) : StatusCode(StatusCodes.Status204NoContent, response);
    }
}

