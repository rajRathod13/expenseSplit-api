
using ExpenseSplit.Domain.Entities;
using MassTransit;

namespace ExpenseSplit.API.Services;

public class MessageService : IMessageService
{
    private readonly ILogger<MessageService> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public MessageService(ILogger<MessageService> logger,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }
    public Task SendNotification(Guid GroupId, string GroupTitle)
    {
        _logger.LogInformation($"Group with id:{GroupId} and title: {GroupTitle} created successfully.");
        _publishEndpoint.Publish(new NotificationRecord(GroupId,GroupTitle));
        return Task.CompletedTask;
    }
}
