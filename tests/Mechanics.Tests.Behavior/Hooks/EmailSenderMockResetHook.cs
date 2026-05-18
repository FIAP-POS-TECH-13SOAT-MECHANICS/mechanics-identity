using Reqnroll;

namespace Mechanics.Tests.Behavior.Hooks;

[Binding]
public class EmailSenderMockResetHook
{
    [BeforeScenario]
    public void ResetEmailSenderMock()
    {
        ApiHook.Factory.EmailSenderMock.Invocations.Clear();
    }
}
