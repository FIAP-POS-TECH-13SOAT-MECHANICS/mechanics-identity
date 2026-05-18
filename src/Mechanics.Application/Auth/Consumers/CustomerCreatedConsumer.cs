using Mechanics.Application.Auth.Event;
using Mechanics.Application.Auth.Requests;
using Mechanics.Application.Auth.Services;
using Mechanics.Infra.Messaging.Consumers;
using Microsoft.Extensions.Logging;

namespace Mechanics.Application.Auth.Consumers;

public class CustomerCreatedConsumer(ILogger<CustomerCreatedConsumer> logger, UserAppService service)
    : IEventConsumer<CustomerCreatedEvent>
{
    public async Task ConsumeAsync(CustomerCreatedEvent message, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating user for customer '{CustomerId}'", message.CustomerId);

        var request = new CreateUserForCustomerRequest(message);
        var response = await service.Create(request, cancellationToken);

        if (response is null)
        {
            logger.LogWarning("Error creating user for customer '{CustomerId}'", message.CustomerId);
            return;
        }

        logger.LogInformation("User for customer '{CustomerId}' created with ID '{UserId}'",
            message.CustomerId, response.CreatedId);
    }
}
