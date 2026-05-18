using Mechanics.Application.Auth.Requests;
using Mechanics.Infra.Security.Models;
using Mechanics.Tests.Behavior.Hooks;
using System.Net.Http.Json;

namespace Mechanics.Tests.Behavior.Drivers;

public class UserManagementDriver
{
    private readonly HttpClient _adminClient = ApiHook.Factory.GetAuthenticatedClient(RoleNames.Administrator);

    public async Task<HttpResponseMessage> CreateUserAsync(CreateUserRequest request)
        => await _adminClient.PostAsJsonAsync("/users", request);

    public static async Task<HttpResponseMessage> ChangePasswordAsync(HttpClient client, ChangePasswordRequest request)
        => await client.PostAsJsonAsync("/auth/change-password", request);
}
