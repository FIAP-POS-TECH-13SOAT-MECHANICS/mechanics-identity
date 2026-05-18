using Mechanics.Application.Auth.Events;
using Mechanics.Application.Auth.Requests;
using Mechanics.Domain.Auth;
using Mechanics.Infra.Data;
using Mechanics.Infra.Data.Seeds;
using Mechanics.Infra.Security.Models;
using Mechanics.Tests.Behavior.Drivers;
using Mechanics.Tests.Behavior.Hooks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Reqnroll;
using System.Net;

namespace Mechanics.Tests.Behavior.Steps;

[Binding]
public class UserManagementSteps(ScenarioContext ctx)
{
    private static readonly Guid AdministratorRoleId =
        RoleSeeds.GetSeeds().First(r => r.Name == RoleNames.Administrator).Id;

    private readonly UserManagementDriver _driver = new();

    [Given(@"que não existe usuário com o CPF ""(.*)""")]
    public async Task GivenQueNaoExisteUsuarioComOCpf(string cpf)
    {
        var normalizedCpf = new string(cpf.Where(char.IsDigit).ToArray());
        using var scope = ApiHook.Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.CpfNumber == normalizedCpf);
        if (user is not null)
        {
            dbContext.Users.Remove(user);
            await dbContext.SaveChangesAsync();
        }

        ctx["cpf"] = cpf;
    }

    [Given(@"que já existe usuário com o CPF ""(.*)""")]
    public async Task GivenQueJaExisteUsuarioComOCpf(string cpf)
    {
        var normalizedCpf = new string(cpf.Where(char.IsDigit).ToArray());
        using var scope = ApiHook.Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var existing = await dbContext.Users.FirstOrDefaultAsync(u => u.CpfNumber == normalizedCpf);
        if (existing is null)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = "USUARIO EXISTENTE",
                CpfNumber = normalizedCpf,
                Email = $"existente_{normalizedCpf}@test.com",
                RoleId = AdministratorRoleId,
                PasswordHash = "placeholder",
                SecurityStamp = Guid.NewGuid().ToString(),
                CreationDate = DateTime.UtcNow,
            };
            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, "Test@1234");
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
        }

        ctx["cpf"] = cpf;
    }

    [Given(@"que existe um usuário com id ""(.*)""")]
    public async Task GivenQueExisteUmUsuarioComId(string idLabel)
    {
        const string currentPassword = "SenhaAtual@2025";
        var userId = Guid.NewGuid();
        ctx["userId"] = userId;
        ctx["currentPassword"] = currentPassword;

        using var scope = ApiHook.Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = new User
        {
            Id = userId,
            FullName = "USUARIO TROCA SENHA",
            CpfNumber = userId.ToString("N")[..11],
            Email = $"troca_{userId:N}@test.com",
            RoleId = AdministratorRoleId,
            PasswordHash = "placeholder",
            SecurityStamp = Guid.NewGuid().ToString(),
            CreationDate = DateTime.UtcNow,
        };
        user.PasswordHash = new PasswordHasher<User>().HashPassword(user, currentPassword);
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
    }

    [When(@"eu envio uma requisição de criação com nome ""(.*)"", CPF ""(.*)"", email ""(.*)"" e role ""(.*)""")]
    public async Task WhenEuEnvioUmaRequisicaoDeCreacao(string nome, string cpf, string email, Guid roleId)
    {
        var request = new CreateUserRequest
        {
            FullName = nome,
            CpfNumber = cpf,
            Email = email,
            RoleId = roleId,
        };
        var response = await _driver.CreateUserAsync(request);
        ctx["response"] = response;
    }

    [When("eu envio uma requisição de criação com o mesmo CPF")]
    public async Task WhenEuEnvioUmaRequisicaoDeCreacaoComOMesmoCpf()
    {
        var cpf = (string)ctx["cpf"];
        var request = new CreateUserRequest
        {
            FullName = "OUTRO USUARIO",
            CpfNumber = cpf,
            Email = $"outro_{Guid.NewGuid()}@test.com",
            RoleId = AdministratorRoleId,
        };
        var response = await _driver.CreateUserAsync(request);
        ctx["response"] = response;
    }

    [When(@"eu altero a senha para ""(.*)""")]
    public async Task WhenEuAlteroASenhaPara(string novaSenha)
    {
        var userId = (Guid)ctx["userId"];
        var currentPassword = (string)ctx["currentPassword"];
        var client = ApiHook.Factory.GetAuthenticatedClientForUser(userId, RoleNames.Administrator);
        var request = new ChangePasswordRequest
        {
            CurrentPassword = currentPassword,
            NewPassword = novaSenha,
        };
        var response = await UserManagementDriver.ChangePasswordAsync(client, request);
        ctx["response"] = response;
        ctx["newPassword"] = novaSenha;
        ctx["changedUserId"] = userId;
    }

    [Then("o usuário é criado")]
    public async Task ThenOUsuarioECriado()
    {
        var response = (HttpResponseMessage)ctx["response"];
        if (response.StatusCode != HttpStatusCode.Created)
        {
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[DEBUG_LOG] Error response: {content}");
        }

        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
    }

    [Then("a resposta é 400 BadRequest")]
    public async Task ThenARespostaE400BadRequest()
    {
        var response = (HttpResponseMessage)ctx["response"];
        if (response.StatusCode != HttpStatusCode.BadRequest)
        {
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[DEBUG_LOG] Error response: {content}");
        }

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Then("a senha é atualizada com hash correto")]
    public async Task ThenASenhaEAtualizadaComHashCorreto()
    {
        var response = (HttpResponseMessage)ctx["response"];
        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);

        var userId = (Guid)ctx["changedUserId"];
        var newPassword = (string)ctx["newPassword"];

        using var scope = ApiHook.Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = await dbContext.Users.FindAsync(userId);
        Assert.IsNotNull(user);

        var result = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, newPassword);
        Assert.AreNotEqual(PasswordVerificationResult.Failed, result);
    }

    [Then("um evento UserChanged é publicado na fila")]
    public void ThenUmEventoUserChangedEPublicadoNaFila()
    {
        ApiHook.Factory.EventPublisherMock.Verify(
            x => x.PublishAsync(It.IsAny<UserChangedEvent>(), It.IsAny<CancellationToken>()),
            Times.AtLeastOnce);
    }

    [Given("existe uma role com o nome {string} e id {string}")]
    public async Task GivenExisteUmaRoleComONomeEId(string roleName, Guid roleId)
    {
        using var scope = ApiHook.Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (dbContext.Roles.Any(role => role.Name == roleName))
            return;

        dbContext.Roles.Add(new Role { Id = roleId, Name = roleName });
        await dbContext.SaveChangesAsync();
    }
}
