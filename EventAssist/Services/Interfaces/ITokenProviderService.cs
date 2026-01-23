using EventAssist.Models.Records;

namespace EventAssist.Services.Interfaces
{
    public interface ITokenProviderService
    {
        string GetToken(UserRecord user);
        string GetTempToken(UserRecord user);
    }
}
