using Mechanics.Domain.Auth;

namespace Mechanics.Application.Notification.Services;

public interface IEmailService
{
    Task SendUserPasswordCreationCode(User user, string passwordCreationCode, CancellationToken cancellationToken = default);
    Task UserPasswordChanged(User user, CancellationToken cancellationToken = default);

    Task SendCustomerUserPasswordCreationCode(User user, string passwordCreationCode,
        CancellationToken cancellationToken = default);
}
