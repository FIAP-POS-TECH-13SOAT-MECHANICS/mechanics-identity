using Mechanics.Application.Notification.Services;
using Mechanics.Domain.Auth;

namespace Mechanics.Tests.Unit.Mocks;

/// <summary>
/// Spy para IEmailService, colocado na pasta Mocks para seguir o padrão do repositório.
/// Permite asserts simples nos testes unitários.
/// </summary>
public class EmailServiceMock : IEmailService
{
    public Task SendUserPasswordCreationCode(User user, string passwordCreationCode,
        CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task UserPasswordChanged(User user, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task SendCustomerUserPasswordCreationCode(User user, string passwordCreationCode,
        CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
}
