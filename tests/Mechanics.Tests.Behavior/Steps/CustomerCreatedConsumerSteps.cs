using Mechanics.Application.Auth.Event;
using Mechanics.Infra.Data;
using Mechanics.Infra.Integrations.EmailSender;
using Mechanics.Infra.Security.Models;
using Mechanics.Tests.Behavior.Drivers;
using Mechanics.Tests.Behavior.Hooks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Reqnroll;

namespace Mechanics.Tests.Behavior.Steps;

[Binding]
public class CustomerCreatedConsumerSteps(ScenarioContext ctx)
{
    [When(@"o evento CustomerCreated é recebido com CPF ""(.*)"" e customerId ""(.*)""")]
    public async Task WhenOEventoCustomerCreatedERecebidoComCpfECustomerId(string cpf, string customerIdLabel)
    {
        var customerId = Guid.NewGuid();
        ctx["customerId"] = customerId;
        ctx["cpf"] = cpf;

        var message = new CustomerCreatedEvent
        {
            FullName = "CLIENTE TESTE",
            CpfNumber = cpf,
            Email = $"cliente_{new string(cpf.Where(char.IsDigit).ToArray())}@test.com",
            CustomerId = customerId,
            IsAdmin = false,
        };

        await CustomerCreatedConsumerDriver.ConsumeAsync(message);
    }

    [When("o evento CustomerCreated é recebido novamente com o mesmo CPF")]
    public async Task WhenOEventoCustomerCreatedERecebidoNovamenteComOMesmoCpf()
    {
        var cpf = (string)ctx["cpf"];
        var normalizedCpf = new string(cpf.Where(char.IsDigit).ToArray());
        var customerId = Guid.NewGuid();

        var message = new CustomerCreatedEvent
        {
            FullName = "CLIENTE DUPLICADO",
            CpfNumber = cpf,
            Email = $"duplicado_{normalizedCpf}@test.com",
            CustomerId = customerId,
            IsAdmin = false,
        };

        await CustomerCreatedConsumerDriver.ConsumeAsync(message);
    }

    [Then(@"um usuário é criado com role ""(.*)""")]
    public async Task ThenUmUsuarioECriadoComRole(string roleLabel)
    {
        var cpf = (string)ctx["cpf"];
        var normalizedCpf = new string(cpf.Where(char.IsDigit).ToArray());

        using var scope = ApiHook.Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var user = await dbContext.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.CpfNumber == normalizedCpf);

        Assert.IsNotNull(user, "Usuário não foi criado.");
        Assert.IsNotNull(user.Role, "Role do usuário não foi carregada.");
        Assert.IsTrue(
            user.Role.Name == RoleNames.CustomerUser || user.Role.Name == RoleNames.CustomerAdmin,
            $"Role esperada era Customer (CustomerUser ou CustomerAdmin), mas foi '{user.Role.Name}'.");
    }

    [Then("o customerId fica associado ao usuário")]
    public async Task ThenOCustomerIdFicaAssociadoAoUsuario()
    {
        var cpf = (string)ctx["cpf"];
        var normalizedCpf = new string(cpf.Where(char.IsDigit).ToArray());
        var customerId = (Guid)ctx["customerId"];

        using var scope = ApiHook.Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.CpfNumber == normalizedCpf);

        Assert.IsNotNull(user, "Usuário não foi encontrado.");
        Assert.AreEqual(customerId, user.CustomerId, "CustomerId não está associado ao usuário.");
    }

    [Then("um e-mail de criação de senha é disparado")]
    public void ThenUmEmailDeCriacaoDeSenhaEDisparado()
    {
        ApiHook.Factory.EmailSenderMock.Verify(
            x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()),
            Times.AtLeastOnce);
    }

    [Then("nenhum usuário adicional é criado")]
    public async Task ThenNenhumUsuarioAdicionalECriado()
    {
        var cpf = (string)ctx["cpf"];
        var normalizedCpf = new string(cpf.Where(char.IsDigit).ToArray());

        using var scope = ApiHook.Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var count = await dbContext.Users.CountAsync(u => u.CpfNumber == normalizedCpf);

        Assert.AreEqual(1, count, $"Esperava apenas 1 usuário com o CPF, mas encontrou {count}.");
    }

    [Then("nenhum e-mail é disparado")]
    public void ThenNenhumEmailEDisparado()
    {
        ApiHook.Factory.EmailSenderMock.Verify(
            x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
