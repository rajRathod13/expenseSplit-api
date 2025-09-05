using ExpenseSplit.Domain.Entities;
using MassTransit;

namespace ExpenseSplit.Consumer.Services;

public class NotificationConsumerService : IConsumer<NotificationRecord>
{
    private readonly ILogger<NotificationConsumerService> _logger;

    public NotificationConsumerService(ILogger<NotificationConsumerService> logger)
    {
        _logger = logger;
    }
    public Task Consume(ConsumeContext<NotificationRecord> context)
    {
        _logger.LogInformation($"Created GroupId : {context.Message.GroupId} with title: {context.Message.GroupTitle}.");
        return Task.CompletedTask;
    }
}
