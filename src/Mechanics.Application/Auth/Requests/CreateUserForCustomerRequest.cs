using Mechanics.Application.Auth.Events;
using Mechanics.Infra.Data.Seeds;
using Mechanics.Infra.Security.Models;
using System.ComponentModel.DataAnnotations;

namespace Mechanics.Application.Auth.Requests;

public class CreateUserForCustomerRequest
{
    /// <summary>
    ///     Nome completo do usuário.
    /// </summary>
    public string FullName { get; }

    /// <summary>
    ///     CPF do usuário.
    /// </summary>
    public string CpfNumber { get; }

    /// <summary>
    ///     E-mail do usuário.
    /// </summary>
    [EmailAddress]
    public string Email { get; }

    /// <summary>
    ///     ID do cliente.
    /// </summary>
    public Guid CustomerId { get; }

    /// <summary>
    ///     Perfil de acesso associado ao usuário.
    /// </summary>
    /// <remarks>Deve ser um perfil válido para clientes.</remarks>
    public Guid RoleId { get; }

    public CreateUserForCustomerRequest(CustomerCreatedEvent message)
    {
        FullName = message.FullName;
        CpfNumber = message.CpfNumber;
        Email = message.Email;
        CustomerId = message.CustomerId;
        RoleId = message.IsAdmin
            ? RoleSeeds.GetSeeds().First(role => role.Name == RoleNames.CustomerAdmin).Id
            : RoleSeeds.GetSeeds().First(role => role.Name == RoleNames.CustomerUser).Id;
    }
}
