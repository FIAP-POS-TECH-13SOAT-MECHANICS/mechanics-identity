using Mechanics.Application.Notification.Services;
using Mechanics.Infra.Integrations.EmailSender;
using Mechanics.Tests.Unit.Mocks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Mechanics.Tests.Unit.Tests.Notifications;

[TestClass]
[TestCategory("Notification")]
[TestCategory("Email")]
public class EmailServiceTests
{
    [TestMethod]
    public async Task It_ShouldSendEmail_WithUserPasswordCreationCode()
    {
        // Arrange
        var senderServiceMock = new Mock<IEmailSenderService>();
        senderServiceMock
            .Setup(s => s.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = CreateInstance(senderServiceMock.Object);
        var mechanic = UserMocks.CreateUser("maria.dev", "123456");
        const string code = "ABCD-1234";

        // Act
        await service.SendUserPasswordCreationCode(mechanic, code, CancellationToken.None);

        // Assert
        senderServiceMock.Verify(s => s.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()), Times.Once);

        var emailMessage = senderServiceMock.Invocations[0].Arguments[0] as EmailMessage;
        Assert.IsNotNull(emailMessage);
        Assert.AreEqual(mechanic.Email, emailMessage.Recipient);
        Assert.IsNotNull(emailMessage.Subject);
        Assert.Contains(code, emailMessage.Body);
    }

    [TestMethod]
    public async Task It_ShouldSendEmail_WithCustomerUserPasswordCreationCode()
    {
        // Arrange
        var senderServiceMock = new Mock<IEmailSenderService>();
        senderServiceMock
            .Setup(s => s.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = CreateInstance(senderServiceMock.Object);
        var user = UserMocks.CreateUser("cliente.teste", "12345678901");
        const string code = "YHLur6lSn";

        // Act
        await service.SendCustomerUserPasswordCreationCode(user, code, CancellationToken.None);

        // Assert
        senderServiceMock.Verify(s => s.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()), Times.Once);

        var emailMessage = senderServiceMock.Invocations[0].Arguments[0] as EmailMessage;
        Assert.IsNotNull(emailMessage);
        Assert.AreEqual(user.Email, emailMessage.Recipient);
        Assert.IsNotNull(emailMessage.Subject);
        Assert.Contains(code, emailMessage.Body);
        Assert.Contains(user.CpfNumber, emailMessage.Body);
        Assert.Contains("ordens de serviço", emailMessage.Body);
    }

    [TestMethod]
    public async Task It_ShouldSendEmail_WhenUserPasswordIsChanged()
    {
        // Arrange
        var senderServiceMock = new Mock<IEmailSenderService>();
        senderServiceMock
            .Setup(s => s.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = CreateInstance(senderServiceMock.Object);
        var user = UserMocks.CreateUser("carlos.teste", "123456");

        // Act
        await service.UserPasswordChanged(user, CancellationToken.None);

        // Assert
        senderServiceMock.Verify(s => s.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()), Times.Once);

        var emailMessage = senderServiceMock.Invocations[0].Arguments[0] as EmailMessage;
        Assert.IsNotNull(emailMessage);
        Assert.AreEqual(user.Email, emailMessage.Recipient);
        Assert.IsNotNull(emailMessage.Subject);
        Assert.IsTrue(emailMessage.Body.Contains("senha", StringComparison.OrdinalIgnoreCase));
    }

    private static EmailService CreateInstance(IEmailSenderService senderService) =>
        new(new NullLoggerFactory().CreateLogger<EmailService>(), senderService);
}
