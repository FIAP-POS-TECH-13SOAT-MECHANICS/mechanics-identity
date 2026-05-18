using Mechanics.Application.Auth.Consumers;
using Mechanics.Application.Auth.Event;
using Mechanics.Domain.Auth;
using Mechanics.Infra.Data;
using Mechanics.Infra.Data.Seeds;
using Mechanics.Infra.Messaging.Consumers;
using Mechanics.Tests.Behavior.Hooks;
using Microsoft.Extensions.DependencyInjection;

namespace Mechanics.Tests.Behavior.Drivers;

public class CustomerCreatedConsumerDriver
{
    public static async Task ConsumeAsync(CustomerCreatedEvent message)
    {
        using var scope = ApiHook.Factory.Services.CreateScope();

        await EnsureRolesExistAsync(scope);

        var consumer = scope.ServiceProvider.GetRequiredService<IEventConsumer<CustomerCreatedEvent>>();
        await consumer.ConsumeAsync(message);
    }

    private static async Task EnsureRolesExistAsync(IServiceScope scope)
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        foreach (var seed in RoleSeeds.GetSeeds())
        {
            if (!dbContext.Roles.Any(r => r.Id == seed.Id))
                dbContext.Roles.Add(new Role { Id = seed.Id, Name = seed.Name });
        }
        await dbContext.SaveChangesAsync();
    }
}
