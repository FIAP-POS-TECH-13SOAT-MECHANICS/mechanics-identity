using Mechanics.Domain.Auth;
using Mechanics.Infra.Data.Seeds;

namespace Mechanics.Application.Auth.Events;

/// <summary>
///     Publicado ao alterar os dados de login do usuário, como <see cref="Role"/> e senha.
/// </summary>
/// <param name="user"></param>
public class UserChangedEvent(User user)
{
    public string Id { get; } = user.Id.ToString();
    public string CpfNumber { get; } = user.CpfNumber;
    public string FullName { get; } = user.FullName;
    public string Role { get; } = RoleSeeds.GetSeeds().First(role => role.Id == user.RoleId).Name;
    public string SecurityStamp { get; } = user.SecurityStamp;
    public string PasswordHash { get; } = user.PasswordHash;
    public string? CustomerId { get; } = user.CustomerId?.ToString();
    public DateTimeOffset LastUpdate { get; } = DateTimeOffset.Now;
}
