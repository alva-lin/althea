namespace Althea.Infrastructure;

public interface IAuthInfoProvider
{
    public void SetCurrentUser(string user);

    public string CurrentUser { get; }
}

public class UnknownAuthInfoProvider : IAuthInfoProvider
{
    public UnknownAuthInfoProvider()
    {
        CurrentUser = "Unknown";
    }

    public void SetCurrentUser(string user)
    {
        CurrentUser = user;
    }

    public string CurrentUser { get; private set; }
}
