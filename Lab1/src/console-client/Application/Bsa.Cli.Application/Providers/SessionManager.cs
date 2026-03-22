namespace Bsa.Cli.Application.Providers;

public sealed class SessionManager
{
    public Guid? CurrentSessionId { get; private set; }

    public void Login(Guid sessionId)
    {
        CurrentSessionId = sessionId;
    }

    public void Logout()
    {
        CurrentSessionId = null;
    }
}