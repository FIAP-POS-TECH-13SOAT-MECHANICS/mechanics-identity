namespace Mechanics.Application.Auth.Events;

public class CustomerCreatedEvent
{
    public required string FullName { get; init; }
    public required string CpfNumber { get; init; }
    public required string Email { get; init; }
    public required Guid CustomerId { get; init; }
    public required bool IsAdmin { get; init; }
}
