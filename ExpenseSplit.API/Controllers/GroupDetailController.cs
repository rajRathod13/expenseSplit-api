using ExpenseSplit.Application.Commands.GroupDetails;
using ExpenseSplit.Application.Queries.GroupDetail;
using ExpenseSplit.Common.RequestDTOs.Group;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplit.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GroupDetailController : BaseController
{
    private readonly ISender _sender;
    //private readonly IMessageService _messageService;

    public GroupDetailController(ISender sender)
    {
        _sender = sender;
        //_messageService = messageService;
    }

    [HttpPost("CreateGroup")]
    [Authorize]
    public async Task<ActionResult> CreateGroup([FromForm] AddGroupRequest request) 
    {
        var response = await  _sender.Send(new AddGroupDetailCommand { AddGroupRequest = request});
        //if (response.IsSuccess)
        //{
        //    await _messageService.SendNotification(response.Data.GroupId, response.Data.Title);
        //}
        return response.IsSuccess ? Ok(response) : StatusCode(StatusCodes.Status500InternalServerError,response);
    }

    [HttpGet("GetGroups")]
    [Authorize]
    public async Task<ActionResult> GetGroups() 
    {
        var response = await _sender.Send(new GetGroupDetailsQuery());
        return response.IsSuccess ? Ok(response) : StatusCode(StatusCodes.Status500InternalServerError, response);
    }

    [HttpGet("GetGroupDetailById")]
    [Authorize]
    public async Task<ActionResult> GetGroupById([FromQuery] Guid groupId) 
    {
        var response = await _sender.Send(new GetGroupDetailByIdQuery { GroupId = groupId });
        return (response.IsSuccess && response.Data != null) ? Ok(response) : StatusCode(StatusCodes.Status204NoContent, response);
    }

    [HttpGet("GetGroupDetailByUserId")]
    [Authorize]
    public async Task<ActionResult> GetGroupByUserId()
    {
        var response = await _sender.Send(new GetGroupDetailsByUserIdQuery());
        return (response.IsSuccess && response.Data != null) ? Ok(response) : StatusCode(StatusCodes.Status204NoContent, response);
    }

    [HttpGet("GetGroupUsers")]
    [Authorize]
    public async Task<ActionResult> GetGroupUsers([FromQuery] Guid groupId) 
    {
        var response = await _sender.Send(new GetGroupUsersByGroupIdQuery { GroupId = groupId });
        return (response.IsSuccess && response.Data != null) ? Ok(response) : StatusCode(StatusCodes.Status204NoContent, response);
    }

    [HttpDelete("DeleteGroup")]
    [Authorize]
    public async Task<ActionResult> DeleteGroup([FromQuery] Guid groupId) 
    {
        var response = await _sender.Send(new DeleteGroupCommand { GroupId = groupId });
        return response.IsSuccess ? Ok(response) : StatusCode(StatusCodes.Status500InternalServerError, response);
    }
}
