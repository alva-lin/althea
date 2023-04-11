namespace Althea.Infrastructure;

public interface IAuthInfoProvider
{
    string CurrentUser { get; }
}

public class UnknownAuthInfoProvider : IAuthInfoProvider
{
    public string CurrentUser => "Unknown";
}