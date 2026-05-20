using Reqnroll;

namespace Mechanics.Tests.Behavior.Hooks;

[Binding]
public class EmailSenderMockResetHook
{
    [BeforeScenario]
    public static void ResetEmailSenderMock()
    {
        ApiHook.Factory.EmailSenderMock.Invocations.Clear();
    }
}
